using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using WalletService.Core;
using WalletService.Core.Repositories;
using CommonConstants = Siccar.Common.Constants;
using WalletService.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Asp.Versioning;

namespace WalletService.V1.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route(CommonConstants.PendingTransactionsAPIURL)]
    [Produces("application/json")]
    public class PendingTransactionsController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<PendingTransactionsController> _logger;

        public PendingTransactionsController(IWalletRepository walletRepository, ILogger<PendingTransactionsController> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        /// <summary>
        /// Adds a transaction id to any recientWallets and removes the previous pendingTx from the senders wallet
        /// </summary>
        /// <param name="transactionConfirmedPayload">TransactionId with destination wallet id in which to update</param>
        /// <returns>Status result</returns>
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Topic(WalletConstants.PubSub, Topics.TransactionConfirmedTopicName)]
        [HttpPost]
        public async Task<ActionResult> UpdatePendingTxsForWallets([FromBody] TransactionConfirmed transactionConfirmedPayload)
        {
            int updates = 0;
            if (transactionConfirmedPayload.MetaData.TransactionType == TransactionTypes.Action ||
                transactionConfirmedPayload.MetaData.TransactionType == TransactionTypes.Rejection)
            {
                foreach (var walletAddress in transactionConfirmedPayload.ToWallets)
                {
                    _logger.LogDebug($"UPDATE {walletAddress} : TX {transactionConfirmedPayload.TransactionId}");
                    updates += await _walletRepository.WalletUpdateTransactions(walletAddress, transactionConfirmedPayload.TransactionId, transactionConfirmedPayload.PreviousTransactionId);
                };
                // we can update the sender the transaction is confirmed if they are not a recipient
                if (!transactionConfirmedPayload.ToWallets.Contains(transactionConfirmedPayload.Sender))
                {
                    _logger.LogDebug($"UPDATE SENDER {transactionConfirmedPayload.Sender} : TX {transactionConfirmedPayload.TransactionId}");
                    updates += await _walletRepository.WalletUpdateTransactions(transactionConfirmedPayload.Sender, transactionConfirmedPayload.TransactionId, transactionConfirmedPayload.PreviousTransactionId);
                }
            }

            return Accepted(updates);
        }

        /// <summary>
        /// Gets a list of received transactions for the specified wallet
        /// </summary>
        /// <param name="address">The public key of the desired wallet to retrieve from storage</param>
        /// <returns>List of received transactions</returns>
        [HttpGet("{address}")]
        public async Task<ActionResult<List<PendingTransaction>>> GetTransactions([FromRoute] string address)
        {
            var wallet = await _walletRepository.GetWallet(address, GetUserSub());

            if (wallet is null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, $"Wallet with address: {address}, could not be found.");
            }

            var rtx = new List<PendingTransaction>();
            foreach (var tx in wallet.Transactions.Where(c => c.isConfirmed && !c.isSpent))
            {
                // we are goign to make a PendingTransaction as thats the way the system currently needs it
                // there needs to be a filter as we have way more stored
                rtx.Add(new PendingTransaction()
                {
                    Id = tx.TransactionId,
                    MetaData = tx.MetaData,
                    TxId = tx.TransactionId!,
                    SenderWallet = tx.Sender!,
                    Timestamp = tx.Timestamp
                });
            }

            return Ok(rtx);
        }

        private string GetUserSub()
        {
            var userSub = Request.HttpContext.User.FindFirst(claim => claim.Type == "sub").Value;

            return userSub;
        }
    }
}
