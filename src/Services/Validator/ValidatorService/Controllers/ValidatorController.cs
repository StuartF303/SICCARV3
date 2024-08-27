// Validator Controller Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System.Data;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Platform;
using Siccar.Registers.ValidatorCore;
#nullable enable

namespace Siccar.Register.ValidatorService.Controllers
{
	[ApiController]
    [Authorize]
    [Route(Constants.ValidatorAPIURL)]
	[Produces("application/json")]
	public class ValidatorController : ControllerBase
	{
		public const string StoreName = "validatorstore";
		public const string PubSub = "pubsub";
		private readonly ILogger<ValidatorController> _logger;
		private readonly IMemPool MemPool;
		private readonly ISiccarValidator validator;

        /// <summary>
        /// ValidatorController Constructor with logger injection
        /// </summary>
        /// <param name="mempool"></param>
        /// <param name="logger"></param>
        /// <param name="validatorHost"></param>
        public ValidatorController(IMemPool mempool, ILogger<ValidatorController> logger, ISiccarValidator validatorHost)
		{
			_logger = logger;
			MemPool = mempool;
			validator = validatorHost;

        }

		/// <summary>
		/// Gets the Mempool information as specified by the id.
		/// </summary>
		/// <param name="transaction">Transaction information for the id from Dapr state store.</param>
		/// <returns>Validator information. ***initial skeleton test only***</returns>
		[HttpGet("{transaction}")]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.RegisterMaintainerRole}")]
        public ActionResult<TransactionModel> Get([FromState(StoreName)] StateEntry<TransactionModel> transaction)
		{
			if (transaction.Value is null)
				return this.NotFound();
			return transaction.Value;
		}
		/// <summary>
		/// Get the Status of the Validator 
		/// </summary>
		/// <returns></returns>
		[HttpGet()]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.RegisterMaintainerRole}")]
        public ActionResult<string> Status()
		{

			var regStatus = validator.Status;
			return Ok(regStatus);
		}

        /// <summary>
        /// Method for persisiting a new transaction state.
        /// </summary>
        /// <param name="transaction">Transaction info.</param>
        /// <returns>A Task representing the result of the operation.</returns>
        /// "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic(PubSub, Topics.TransactionPendingTopicName)]
		[HttpPost]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        public Task ReceiveTx([FromBody] TransactionModel transaction)
		{
			_logger.LogDebug("Received New Transaction : {regId} : {txId} ", transaction.MetaData?.RegisterId, transaction.TxId);
			MemPool.AddTxToPool(transaction);
			return Task.CompletedTask;
		}
	}
}