using Asp.Versioning;
using BlueprintService.V1.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Application.HelperFunctions;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlueprintService.V1.Controllers
{
	[ApiController]
	[Authorize]
	[ApiVersion("1.0")]
	[Route(Constants.BlueprintAPIURL)]
	public class BlueprintsController : ControllerBase
	{
		private readonly ILogger<BlueprintsController> _logger;
		private readonly IBlueprintRepository _blueprintRepository;
		private readonly IWalletServiceClient _walletServiceClient;
		private readonly IRegisterServiceClient _registerServiceClient;
		private readonly ITenantServiceClient _tenantServiceClient;
		private readonly IValidator<Blueprint> _validator;
		private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

		public BlueprintsController(ILogger<BlueprintsController> logger,
			IBlueprintRepository blueprintRepository,
			IWalletServiceClient walletServiceClient,
			IRegisterServiceClient registerServiceClient,
			ITenantServiceClient tenantServiceClient,
			IValidator<Blueprint> validator)
		{
			_logger = logger;
			_blueprintRepository = blueprintRepository;
			_walletServiceClient = walletServiceClient;
			_registerServiceClient = registerServiceClient;
			_tenantServiceClient = tenantServiceClient;
			_validator = validator;
		}

		[HttpGet]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole},{Constants.BlueprintAuthoriserRole}")]
		public async Task<ActionResult<IEnumerable<Blueprint>>> GetAll()
		{
			var blueprints = await _blueprintRepository.GetAll();
			return Ok(blueprints);
		}

		[HttpGet("{registerId}/{blueprintId}/published")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole},{Constants.BlueprintAuthoriserRole}")]
		public async Task<ActionResult<Blueprint>> GetPublishedBlueprint(string registerId, string blueprintId, [FromQuery(Name = "wallet-address")] string walletAddress)
		{
			var transaction = await _registerServiceClient.GetPublishedBlueprintTransaction(registerId, blueprintId);
			//var latestBlueprintTransaction = BlueprintHelperFunctions.GetLatestBlueprintTransactions(new List<RawTransaction>{ transaction }).FirstOrDefault();
			return await GetBlueprintFromTransaction(transaction, walletAddress);
		}

		[HttpGet]
		[Route("{id}")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole},{Constants.BlueprintAuthoriserRole}")]
		public async Task<ActionResult<IEnumerable<Blueprint>>> Get([FromRoute] string id)
		{
			var blueprint = await _blueprintRepository.GetBlueprint(id);
			return Ok(blueprint);
		}

		[HttpPost]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole}")]
		public async Task<IActionResult> Save([FromBody] Blueprint blueprint)
		{
			ValidationResult result = await _validator.ValidateAsync(blueprint);
			if (!result.IsValid)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				_logger.LogError("Blueprint Model failed validation: {state}", ModelState);
				return BadRequest(ModelState);
			}
			await _blueprintRepository.SaveBlueprint(blueprint);
			return Accepted(blueprint);
		}

		[HttpPost("{walletAddress}/{registerId}/publish")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole}")]
		public async Task<IActionResult> Publish([FromRoute] string walletAddress, [FromRoute] string registerId, [FromBody] Blueprint blueprint)
		{
			ValidationResult result = await _validator.ValidateAsync(blueprint, options => options.IncludeAllRuleSets());
			if (!result.IsValid)
			{
				_logger.LogInformation($"Publish Blueprint failed validation, Blueprint ID: {blueprint.Id}");
				foreach (var error in result.Errors)
					ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				return BadRequest(ModelState);
			}
			var tenantClaim = Request.HttpContext.User.Claims.LastOrDefault(claim => claim.Type == "tenant");
            if (tenantClaim is null)
			{
				_logger.LogWarning("User missing Tenant Claim");
                throw new HttpStatusException(System.Net.HttpStatusCode.Forbidden, $"Tenant Claim Missing.");
            }
                
            var tenant = await _tenantServiceClient.GetTenantById(tenantClaim.Value);
			if (!tenant.Registers.Contains(registerId))
			{
                _logger.LogWarning($"User Tenant is not authorized to access register: {registerId}");
                throw new HttpStatusException(System.Net.HttpStatusCode.Forbidden, $"User Tenant is not authorized to access register: {registerId}");
			}
			//Get blueprints
			var bps = await _registerServiceClient.GetBlueprintTransactions(registerId);
			var (version, prevTransId) = BlueprintHelperFunctions.GetLatestBlueprintVersionAndPrevTransId(blueprint.Id, bps);
			blueprint.Version = version;

			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			tx.SetPrevTxHash(prevTransId);
			tx.SetTxMetaData(JsonSerializer.Serialize(
				new TransactionMetaData { BlueprintId = blueprint.Id, TransactionType = TransactionTypes.Blueprint, RegisterId = registerId, NextActionId = 1 }));
			tx.GetTxPayloadManager().AddPayload(JsonSerializer.SerializeToUtf8Bytes(blueprint));
			TransactionModel transaction = await _walletServiceClient.SignAndSendTransaction(tx.GetTxTransport().transport, walletAddress);

			_logger.LogDebug("Delete drafts");
			if (await _blueprintRepository.BluerpintExists(blueprint.Id))
				await _blueprintRepository.DeleteBlueprint(blueprint.Id);
			_logger.LogInformation($"Published Blueprint {blueprint.Id}");
			return Accepted(transaction);
		}

		[HttpGet("{registerId}/published")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole},{Constants.BlueprintAuthoriserRole}")]
		public async Task<ActionResult<List<Blueprint>>> GetAllPublished([FromRoute] string registerId, [FromQuery(Name = "wallet-address")] string walletAddress)
		{
			var transactions = await _registerServiceClient.GetBlueprintTransactions(registerId);
			var latestBlueprintTransactions = BlueprintHelperFunctions.GetLatestBlueprintTransactions(transactions);
			var tasks = latestBlueprintTransactions.Select(transaction => GetBlueprintFromTransaction(transaction, walletAddress));
			var blueprints = await Task.WhenAll(tasks);
			return Ok(blueprints.ToList());
		}

		private async Task<Blueprint> GetBlueprintFromTransaction(TransactionModel transaction, string walletAddress)
		{
			byte[][] blueprintBytes = string.IsNullOrEmpty(walletAddress) ? await _walletServiceClient.GetAccessiblePayloads(transaction) :
				await _walletServiceClient.DecryptTransaction(transaction, walletAddress);
			var jsonString = Encoding.UTF8.GetString(blueprintBytes[0]);
			return JsonSerializer.Deserialize<Blueprint>(jsonString, _jsonSerializerOptions);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole}")]
		public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Blueprint blueprint)
		{
			if (id != blueprint.Id)
				throw new HttpStatusException(HttpStatusCode.BadRequest, "Blueprint id in url does not match Blueprint id in the body.");

			ValidationResult result = await _validator.ValidateAsync(blueprint);
			if (!result.IsValid)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				return BadRequest(ModelState);
			}
			await _blueprintRepository.UpdateBlueprint(blueprint);
			return Accepted(blueprint);
		}

		[HttpDelete]
		[Route("{id}")]
		[Authorize(Roles = $"{Constants.BlueprintAdminRole}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			await _blueprintRepository.DeleteBlueprint(id);
			return Ok();
		}
	}
}
