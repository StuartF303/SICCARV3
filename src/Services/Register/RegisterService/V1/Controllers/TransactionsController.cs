using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using Siccar.Registers.Core;
using Siccar.Registers.RegisterService.V1.Adapters;
using Siccar.Registers.RegisterService.V1.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace Siccar.Registers.RegisterService.V1
{
    /// <summary>
    /// API to create, control, query and act upon Registers 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route(Constants.RegisterAPIURL)]
    [SwaggerTag("Transactions are the envelopes containing truths as they are shared", externalDocsUrl: "https://siccar.net/docs/Transactions")]
    public class TransactionsController : ODataController
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        public const string PubSub = "pubsub";
        private readonly ILogger<TransactionsController> Log;
        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterResolver _registerResolver;
        private readonly IDaprClientAdaptor _daprClient;
        private readonly IHubContextAdapter _hubContext;

        public TransactionsController(ILogger<TransactionsController> logger, IRegisterRepository registerRepository, IRegisterResolver registerResolver, IDaprClientAdaptor daprClient,
            IHubContextAdapter hubContext)
        {
            Log = logger;
            _registerRepository = registerRepository;
            _registerResolver = registerResolver;
            _daprClient = daprClient;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gets the Transactions as specified by the RegsiterId.
        /// This could be alot of data so needs to be properly paged
        /// </summary>
        /// <param name="RegisterId">Register Identifier</param>
        /// <returns>Register information. ***initial skeleton test only***</returns>
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("({RegisterId})/Transactions")]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<IQueryable<TransactionModel>>> GetTransactions([FromRoute] string RegisterId)
        {
            await _registerResolver.ThrowIfUserNotAuthorizedForRegister(Request.HttpContext.User.Claims, RegisterId);

            return (String.IsNullOrEmpty(RegisterId) || (!await _registerRepository.IsLocalRegisterAsync(RegisterId))) ?
				NotFound() : Ok((await _registerRepository.GetTransactionsAsync(RegisterId)));
        }

        /// <summary>
        /// Gets the Transactions as specified by the RegsiterId.
        /// This could be alot of data so needs to be properly paged
        /// </summary>
        /// <param name="RegisterId">Register Identifier</param>
        /// <returns>Register information. ***initial skeleton test only***</returns>
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("{RegisterId}/Transactions")]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<IQueryable<TransactionModel>>> GetTransactionsClassic([FromRoute] string RegisterId)
        {
            await _registerResolver.ThrowIfUserNotAuthorizedForRegister(Request.HttpContext.User.Claims, RegisterId);

            return (String.IsNullOrEmpty(RegisterId) || (!await _registerRepository.IsLocalRegisterAsync(RegisterId))) ?
                NotFound() : Ok(await _registerRepository.GetTransactionsAsync(RegisterId));
        }

        /// <summary>
        /// Gets the Transactions as specified by the RegsiterId.
        /// This could be alot of data so needs to be properly paged
        /// </summary>
        /// <param name="RegisterId">Register Identifier</param>
        /// <param name="TransactionId">The specfic Transaction Id</param>
        /// <returns>A Specified Transaction</returns>
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("{RegisterId}/Transactions/{TransactionId}")]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<TransactionModel>> GetTransaction([FromRoute] string RegisterId, [FromRoute] string TransactionId)
        {
            await _registerResolver.ThrowIfUserNotAuthorizedForRegister(Request.HttpContext.User.Claims, RegisterId);

            if (String.IsNullOrEmpty(RegisterId) || (!await _registerRepository.IsLocalRegisterAsync(RegisterId)))
                return this.NotFound();
            var t = await _registerRepository.GetTransactionAsync(RegisterId, TransactionId);
            return t != null ? Ok(t) : NotFound();
        }

        /// <summary>
        /// Gets the Transaction information as specified by the id.
        /// </summary>
        /// <param name="RegisterId">Register Identifier</param>
        /// <param name="DocketId">Docket Identifier</param>
        /// <returns>A List of Transaction from the ggiven docket</returns>
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("{RegisterId}/Docket/{DocketId}/Transactions")]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<TransactionModel>> GetTransactionsByDocket(string RegisterId, string DocketId)
        {
            await _registerResolver.ThrowIfUserNotAuthorizedForRegister(Request.HttpContext.User.Claims, RegisterId);

            if (String.IsNullOrEmpty(RegisterId) || String.IsNullOrEmpty(DocketId) || (!await _registerRepository.IsLocalRegisterAsync(RegisterId)))
                return this.NotFound();
            // todo: this needs to become a query...
            return Ok(await _registerRepository.GetTransactionsAsync(RegisterId));
        }
 
        /// <summary>
        /// Method for persisiting a new transaction state from Validation Service.
        /// </summary>
        /// <param name="validatedTransaction">Body should contain a JSON Transaction candidate</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic(PubSub, Topics.TransactionValidationCompletedTopicName)]
        [Topic(PubSub, Topics.TransactionSubmittedTopicName)]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [HttpPost("Transactions", Name = "RemoteTransaction", Order = 2)]
        public async Task<ActionResult<TransactionModel>> PostRemoteTransaction([FromBody] TransactionModel validatedTransaction)
        {
            try
            {
                // do a whole bunch of checks....
                var storedTransaction = await _registerRepository.InsertTransactionAsync(validatedTransaction);
                var transactionConfirmedPayload = new TransactionConfirmed
                {
                    ToWallets = validatedTransaction.RecipientsWallets.ToList(),
                    Sender = validatedTransaction.SenderWallet,
                    PreviousTransactionId = validatedTransaction.PrevTxId,
                    TransactionId = validatedTransaction.TxId,
                    MetaData = validatedTransaction.MetaData
                };

                await _daprClient.PublishEventAsync(PubSub, Topics.TransactionConfirmedTopicName, transactionConfirmedPayload);
                await _hubContext.SendToGroupAsync(validatedTransaction.MetaData!.RegisterId,
                    Topics.TransactionValidationCompletedTopicName, transactionConfirmedPayload);
                return storedTransaction;
            }
            catch (Exception er)
            {
                Log.LogError("FAILED    {msg}", er.Message);
                return BadRequest();
            }
        }
    }
}
