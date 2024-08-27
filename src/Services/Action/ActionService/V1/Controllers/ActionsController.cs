using ActionService.V1.Adaptors;
using ActionService.V1.Factories;
using ActionService.V1.Services;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Dapr;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Application.HelperFunctions;
using Siccar.Application.Validation;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Action = Siccar.Application.Action;
#nullable enable

namespace ActionService.V1.Controllers
{
	[ApiController]
	[Authorize]
	[ApiVersion("1.0")]
	[Route(Constants.ActionAPIURL)]
	[Produces("application/json")]
	[SwaggerTag("The Action Service handles Participant data where intereacting is controlled by a Blueprint", externalDocsUrl: "https://siccar.net/docs/Action")]
	public class ActionsController : ControllerBase
	{
		private readonly IActionService _actionService;
		private readonly IRegisterServiceClient _registerServiceClient;
		private readonly IWalletServiceClient _walletServiceClient;
		private readonly ITenantServiceClient _tenantServiceClient;
		private readonly IPayloadResolver _payloadResolver;
		private readonly IActionResolver _actionResolver;
		private readonly IHubContextAdaptor _actionsHub;
		private readonly ISchemaDataValidator _schemaDataValidator;
		private readonly ISustainabilityHttpServiceClient _sustainabilityHttpService;
		private readonly ITransactionRequestBuilder _transactionRequestBuilder;
		private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
		private readonly ILogger<ActionsController> _logger;

		public ActionsController(IActionService actionService, IRegisterServiceClient registerServiceClient, IWalletServiceClient walletServiceClient,
			ITenantServiceClient tenantServiceClient, IPayloadResolver payloadResolver, IActionResolver actionResolver, IHubContextAdaptor actionsHub,
			ISchemaDataValidator schemaDataValidator, ISustainabilityHttpServiceClient sustainabilityHttpServiceClient, 
			ITransactionRequestBuilder transactionRequestBuilder, ILogger<ActionsController> logger)
		{
			_actionService = actionService;
			_registerServiceClient = registerServiceClient;
			_walletServiceClient = walletServiceClient;
			_tenantServiceClient = tenantServiceClient;
			_payloadResolver = payloadResolver;
			_actionResolver = actionResolver;
			_actionsHub = actionsHub;
			_schemaDataValidator = schemaDataValidator;
			_sustainabilityHttpService = sustainabilityHttpServiceClient;
			_transactionRequestBuilder = transactionRequestBuilder;
			_logger = logger;
		}

		/// <summary>
		/// Get My list of Blueprints I can start.
		/// </summary>
		/// <remarks>
		/// These are the first action in the blueprint.
		/// </remarks>
		/// <example>
		///  {[
		///     {}
		///  ]}
		/// </example>
		/// <returns>A list of my first actions I can start</returns> 
		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpGet("{walletAddress}/{registerId}/blueprints")]
		public async Task<IEnumerable<Action>> GetStartingActions([FromRoute] string registerId, [FromRoute] string walletAddress)
		{
			var transactions = await _registerServiceClient.GetBlueprintTransactions(registerId);
			var latestBlueprintTransactions = BlueprintHelperFunctions.GetLatestBlueprintTransactions(transactions);
			var tasks = latestBlueprintTransactions.Select(async transaction =>
			{
				var blueprintBytes = await _walletServiceClient.DecryptTransaction(transaction, walletAddress);
				var jsonString = Encoding.UTF8.GetString(blueprintBytes[0]) ?? "{}";
				var blueprint = JsonSerializer.Deserialize<Blueprint>(jsonString, _jsonSerializerOptions);
				blueprint!.Id = transaction.Id;
				return blueprint;
			});

			var blueprints = await Task.WhenAll(tasks);
			var ownedWallets = await _walletServiceClient.ListWallets();
			var actionsThatWalletAddressCanStart = new List<Action>();
			foreach(var bp in blueprints)
			{
				var action = bp.Actions.Find(action => action.Id == 1) ?? throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "First action with id 1 could not be found in blueprint actions.");
				action.Blueprint = bp.Id;
				action.PreviousTxId = bp.Id;
				var participant = bp.Participants.Find(participant => participant.WalletAddress == walletAddress);
				if (participant != null && action.Sender == participant.Id)
					actionsThatWalletAddressCanStart.Add(action);
			}
			return actionsThatWalletAddressCanStart;
		}

		/// <summary>
		/// Get My List of Actions
		/// </summary>
		/// <remarks>
		/// Our Actions are our outstanding transactions in the wallet.
		/// We will want to Query/Filter, Sort and only want summary data, not the payloads
		/// Use By ID to pull payloads so we dont decrypt to much data to early
		/// </remarks>
		/// <example>
		///  {[
		///     {}
		///  ]}
		/// </example>
		/// <returns>A list of my actions</returns>
		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpGet("{walletAddress}/{registerId}")]
		public async Task<List<Action>> GetAllAsync([FromRoute] string registerId, [FromRoute] string walletAddress)
		{
			var pendingTxs = await _walletServiceClient.GetWalletTransactions(walletAddress);
			//Reverse the transactions to have the latest first.
			pendingTxs.Reverse();
			var blueprintIdToBlueprintMap = new Dictionary<string, Blueprint>();
			var actions = new List<Action>();
			foreach (var pendingTx in pendingTxs)
			{
				if (!blueprintIdToBlueprintMap.Keys.ToList().Any(blueprintTxId => blueprintTxId == pendingTx.MetaData!.BlueprintId))
				{
					var bpTx = await _registerServiceClient.GetTransactionById(registerId: pendingTx.MetaData!.RegisterId, txId: pendingTx.MetaData.BlueprintId);
					var blueprint = await ResolveBlueprint(walletAddress, bpTx);
					blueprintIdToBlueprintMap[pendingTx.MetaData.BlueprintId] = blueprint;
				}
				var bp = blueprintIdToBlueprintMap[pendingTx.MetaData!.BlueprintId];
				var action = FindActionForPendingTx(pendingTx, bp);
				actions.Add(action);
			}
			return actions;
		}

		private async Task<Blueprint> ResolveBlueprint(string walletAddress, TransactionModel blueprintTransaction)
		{
			var blueprintBytes = await _walletServiceClient.DecryptTransaction(blueprintTransaction, walletAddress);
			var jsonString = Encoding.UTF8.GetString(blueprintBytes[0]);
			var blueprint = JsonSerializer.Deserialize<Blueprint>(jsonString, _jsonSerializerOptions);
			blueprint!.Id = blueprintTransaction.Id;
			return blueprint;
		}

		private static Action FindActionForPendingTx(PendingTransaction pendingTx, Blueprint blueprint)
		{
			var action = blueprint.Actions.Find(action => action.Id == pendingTx.MetaData!.NextActionId) ??
				throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, $"First action with id {pendingTx.MetaData!.NextActionId} could not be found in blueprint actions.");
			var newAction = ActionFactory.BuildAction(action, blueprint.Id, pendingTx.Id, pendingTx.MetaData!.TrackingData);
			return newAction;
		}

		/// <summary>
		/// Get Action by Id
		/// </summary>
		/// <remarks>
		/// The work horse of the Action Service, 
		/// - Blueprint document provides plan 
		///     - Participants(Addresses)
		///     - DataDefinitions
		///     - and Actions of which we are 
		/// - Transaction MetaData provides state
		/// - Payloads that have been decrypted provides values 
		/// </remarks>
		/// <example>
		/// {
		///  "id": "",
		///  "title": "Apply",
		///  "description": "Get my endorsement",
		///  "participants": {
		///    "fb85aa15-0178-46f1-89d7-aeb410812ee3": {}
		///  },
		///  "form": {
		///    "type": "Layout",
		///    "title": "",
		///    "scope": "",
		///    "layout": "VerticalLayout",
		///    "elements": [
		///      {
		///        "type": "TextLine",
		///        "title": "New Title",
		///        "scope": {
		///             "schemaId":"https://siccar.net/schema/person.json",
		///             "schemaProperty":"firstname"
		///             },
		///        "layout": "VerticalLayout"
		///      },
		///      {
		///        "type": "TextLine",
		///        "title": "New Title",
		///        "scope": {
		///             "schemaId":"https://siccar.net/schema/person.json",
		///             "schemaProperty":"surname"
		///             },
		///        "layout": "VerticalLayout"
		///      }
		///    ]
		///  }
		/// }
		/// </example>
		/// <param name="walletAddress">The wallet address to use in encryption/decryption.</param>
		/// <param name="registerId">The register id of that the target transaction exists on.</param>
		/// <param name="transactionId">Previous transaction ID.</param>
		/// <param name="aggregatePreviousTransactionData">Whether the previous transaction data should be aggregated with the current transaction</param>
		/// <returns>a single Action</returns>
		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpGet("{walletAddress}/{registerId}/{transactionId}")]
		public async Task<Action> GetById([FromRoute] string walletAddress, [FromRoute] string registerId, [FromRoute] string transactionId, [FromQuery] bool aggregatePreviousTransactionData = true)
		{
			TransactionModel transaction;
			try
			{
				transaction = await _registerServiceClient.GetTransactionById(registerId, transactionId);
			}
			catch (Siccar.Common.Exceptions.HttpStatusException er) when (er.Status == HttpStatusCode.NotFound) { return null!; }
			var action = await _actionService.GetAction(transaction, walletAddress, registerId, transactionId, aggregatePreviousTransactionData);
			return action;
		}

		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpGet("{walletAddress}/{registerId}/{transactionId}/sustainability")]
		public async Task<SustainabilityModel> GetByIdHack([FromRoute] string walletAddress, [FromRoute] string registerId, [FromRoute] string transactionId, [FromQuery] bool aggregatePreviousTransactionData = true)
		{
			TransactionModel transaction;
			try
			{
				transaction = await _registerServiceClient.GetTransactionById(registerId, transactionId);
			}
			catch (Siccar.Common.Exceptions.HttpStatusException er) when (er.Status == HttpStatusCode.NotFound) { return null!; }

			var action = await _actionService.GetSubmittedActionData(transaction.MetaData!.ActionId, walletAddress, transaction);
			var sus = new SustainabilityModel(action);
			return sus;
		}

		/// <summary>
		/// Accepts the client data response
		/// </summary>
		/// <remarks>
		/// We now take the action response and place into payloads for the
		/// Wallets to encrypt and send
		/// </remarks>
		/// <param name="actionSubmission">Contains all the data for submitting an action.</param>
		/// <returns></returns>
		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpPost]
		[RequestSizeLimit(4000000)]
		public async Task<ActionResult<TransactionModel>> PostResponse([FromBody] ActionSubmission actionSubmission)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			var tenantClaim = Request.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "tenant");
			var tenant = await _tenantServiceClient.GetTenantById(tenantClaim!.Value);
			if (!tenant.Registers.Contains(actionSubmission.RegisterId))
				throw new HttpStatusException(System.Net.HttpStatusCode.Forbidden, $"User Tenant is not authorized to access register: {actionSubmission.RegisterId}");
			_logger.LogDebug($"Processed Tenant Claims in {stopwatch.ElapsedMilliseconds} ms");
			stopwatch.Restart();

			var blueprintId = actionSubmission.BlueprintId ?? throw new HttpStatusException(HttpStatusCode.BadRequest, nameof(actionSubmission.BlueprintId));
			var blueprint = await GetBlueprintAsync(actionSubmission.BlueprintId, actionSubmission.WalletAddress, actionSubmission.RegisterId);
            _logger.LogDebug($"Get Blueprint in {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();

            Action action;
			Transaction transaction;
			if (actionSubmission.PreviousTxId != actionSubmission.BlueprintId)
			{
				var previousActionTx = await _registerServiceClient.GetTransactionById(registerId: actionSubmission.RegisterId, txId: actionSubmission.PreviousTxId);
				action = blueprint.Actions.FirstOrDefault(action => action.Id == previousActionTx.MetaData!.NextActionId)!;
				transaction = await _transactionRequestBuilder.BuildTransactionRequestFromPreviousTransaction(blueprint, actionSubmission, previousActionTx);
                _logger.LogDebug($"Processed Action where previous TX is ActionTX {stopwatch.ElapsedMilliseconds} ms");
            }
			else
			{
				action = blueprint.Actions.FirstOrDefault(action => action.Id == 1)!;
				transaction = await _transactionRequestBuilder.BuildTransactionRequest(blueprint, actionSubmission);
                _logger.LogDebug($"Processed Action where previous TX is Blueprint {stopwatch.ElapsedMilliseconds} ms");
                
            }
            stopwatch.Restart();
            var (isValid, validationMessage) = _schemaDataValidator.ValidateSchemaData(action!.DataSchemas!.First().RootElement.GetRawText(), actionSubmission.Data!.RootElement.GetRawText());
			if (!isValid)
				return BadRequest(validationMessage);
			var tx = await _walletServiceClient.SignAndSendTransaction(transaction, actionSubmission.WalletAddress);
            _logger.LogDebug($"Processed Action Sign&Send {stopwatch.ElapsedMilliseconds} ms");
			stopwatch.Stop();
            return Accepted(tx);
		}

		/// <summary>
		/// Accepts the client data response
		/// </summary>
		/// <remarks>
		/// We now take the action response and place into payloads for the
		/// Wallets to encrypt and send
		/// </remarks>
		/// <param name="rejectSubmission">Contains all the data for submitting an action.</param>
		/// <returns></returns>
		[Authorize(Roles = Constants.WalletUserRole)]
		[HttpPost("reject")]
		public async Task<ActionResult<TransactionModel>> RejectAction([FromBody] ActionSubmission rejectSubmission)
		{
			var previousActionTx = await _registerServiceClient.GetTransactionById(registerId: rejectSubmission.RegisterId, txId: rejectSubmission.PreviousTxId);
			if (previousActionTx.MetaData!.TransactionType == TransactionTypes.Rejection)
				throw new HttpStatusException(HttpStatusCode.BadRequest, "Cannot reject an action which has already been submitted.");

			if (previousActionTx.MetaData.TransactionType != TransactionTypes.Action)
				throw new HttpStatusException(HttpStatusCode.BadRequest, "Cannot reject starting actions.");

			TransactionMetaData meta_data = new()
			{
				BlueprintId = previousActionTx.MetaData.BlueprintId,
				InstanceId = previousActionTx.MetaData.InstanceId,
				ActionId = previousActionTx.MetaData.NextActionId,
				NextActionId = previousActionTx.MetaData.ActionId,
				RegisterId = rejectSubmission.RegisterId,
				TransactionType = TransactionTypes.Rejection,
				TrackingData = previousActionTx.MetaData.TrackingData
			};
			ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			transaction.SetPrevTxHash(rejectSubmission.PreviousTxId);
			transaction.SetTxRecipients(new string[] { previousActionTx.SenderWallet });
			transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
			transaction.GetTxPayloadManager().AddPayload(JsonSerializer.SerializeToUtf8Bytes(rejectSubmission.Data), new string[] { previousActionTx.SenderWallet });
			var tx = await _walletServiceClient.SignAndSendTransaction(transaction.GetTxTransport().transport, rejectSubmission.WalletAddress);
			return Accepted(tx);
		}

		private async Task<Blueprint> GetBlueprintAsync(string txId, string walletAddress, string registerId)
		{
			//TODO we should use the BlueprintService to get the blueprint.
			var blueprintTx = await _registerServiceClient.GetTransactionById(registerId: registerId, txId: txId);
			var blueprintBytes = await _walletServiceClient.DecryptTransaction(blueprintTx, walletAddress);
			var blueprintStr = Encoding.UTF8.GetString(blueprintBytes[0]);
			var blueprint = JsonSerializer.Deserialize<Blueprint>(blueprintStr, _jsonSerializerOptions);
			return blueprint!;
		}

		/// <summary>
		/// Listens for inbound transaction to wallet and announces to action w/ metadata
		/// </summary>
		/// <param name="transactionConfirmedPayload">TransactionId with destination wallet id in which to update</param>
		/// <returns>Status result</returns>
		[Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[Topic("pubsub", Topics.TransactionConfirmedTopicName)]
		[HttpPost("notify")]
		public async Task<ActionResult> NotifyClientsOfNewTransaction([FromBody] TransactionConfirmed transactionConfirmedPayload)
		{
			if (transactionConfirmedPayload.MetaData.TransactionType == TransactionTypes.Action)
			{
				foreach (var wallet in transactionConfirmedPayload.ToWallets)
					await _actionsHub.SendToGroupAsync(wallet, "ReceiveAction", transactionConfirmedPayload);
				//TODO: Find out which actions hub method makes most sense for client use.
				if (!transactionConfirmedPayload.ToWallets.Any())
					await _actionsHub.SendToAllAsync("ReceiveAction", transactionConfirmedPayload);
				await _sustainabilityHttpService.SendPostRequest(transactionConfirmedPayload);
			}
			return Accepted();
		}
	}
}
