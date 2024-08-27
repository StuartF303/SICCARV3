using System;
using Microsoft.AspNetCore.Mvc;
using Siccar.Application;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform.Tenants.Repository;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Client = Siccar.Platform.Tenants.Core.Client;
using Dapr;
using Siccar.Platform.Registers.Core.Models;
using System.Text;
using TenantRepository;
using Siccar.Platform.Cryptography;
using Asp.Versioning;
#nullable enable

namespace Siccar.Platform.Tenants.V1.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/tenants")]
	[Produces("application/json")]
	[SwaggerTag("The Tenants Service handles Organisational User management", externalDocsUrl: "https://docs.siccar.dev/#0d3f2503-7816-4d50-863b-591a507a9b53")]
	public class TenantsController : ControllerBase
	{
		private readonly ITenantRepository _tenantRepository;
		private readonly IWalletServiceClient _walletServiceClient;
		private readonly IRegisterServiceClient _registerServiceClient;
		private readonly IUserRepository _userRepository;

		public TenantsController(ITenantRepository tenantRepository, IWalletServiceClient walletServiceClient, IRegisterServiceClient registerServiceClient,
						IUserRepository userRepository)
		{
			_tenantRepository = tenantRepository;
			_walletServiceClient = walletServiceClient;
			_registerServiceClient = registerServiceClient;
			_userRepository = userRepository;
		}

		/// <summary>
		/// Tenants - Gets all Tenants 
		/// </summary>
		/// <returns>Status code for operation and specified tenant</returns>
		[HttpGet]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole},{Constants.TenantBillingRole}")]
		public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
		{
			var tenantList = _tenantRepository.All<Tenant>().Result.ToList();
			foreach (var tenant in tenantList)
			{
				var clientList = await _tenantRepository.Where<Client>(k => k.TenantId == tenant.Id);
				var usersCount = await _userRepository.ListByTenant(tenant.Id!);
				tenant.Clients = clientList.ToList();
				tenant.AccountsCount = usersCount.Count;
			}
			return Ok(tenantList);
		}

		/// <summary>
		/// Tenants - Gets a Tenant based on the supplied key
		/// </summary>
		/// <returns>Status code for operation and specified tenant</returns>
		[HttpGet("{key}", Name = "GetTenantsByAddress", Order = 3)]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole},{Constants.TenantBillingRole}")]
		public async Task<ActionResult<Tenant>> GetTenantById(string key)
		{
			var tenant = await _tenantRepository.Single<Tenant>(k => k.Id == key);
			if (tenant == null)
				return NotFound();
			var clientList = await _tenantRepository.Where<Client>(k => k.TenantId == tenant.Id);
			tenant.Clients = clientList.ToList();
			return Ok(tenant);
		}

		/// <summary>
		/// Tenants - Creates a Tenant based on the supplied data
		/// </summary>
		/// <returns>Status code for operation and Created tenant</returns>
		[HttpPost]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.InstallationAdminRole}")]
		public async Task<IActionResult> Post([FromBody] Tenant tenant)
		{
			await _tenantRepository.Add(tenant);
			return Accepted(tenant);
		}

		/// <summary>
		/// Tenants - Updates a Tenant based on the supplied data
		/// </summary>
		/// <returns>Status code for operation and updated tenant</returns>
		[HttpPut("{key}")]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole}")]
		public async Task<IActionResult> UpdateTenant(string key, [FromBody] Tenant tenant)
		{
			if (key != tenant.Id)
				throw new HttpStatusException(HttpStatusCode.BadRequest, "Tenant id in url does not match Tenant id in the body.");
			await _tenantRepository.UpdateTenant(tenant);
			return Accepted(tenant);
		}

		/// <summary>
		/// Tenants - Deletes a Tenant based on the supplied data
		/// </summary>
		/// <returns>Status code for operation</returns>
		[HttpDelete("{key}")]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.TenantAdminRole}")]
		public async Task<IActionResult> Delete([FromRoute] string key)
		{
			await _tenantRepository.Delete<Tenant>(t => t.Id == key);
			return Ok();
		}

		/// <summary>
		/// Allow and Admin to Publish a Tenants Participant to a Register
		/// Additional override to publish from a given walletAddress
		/// </summary>
		/// <param name="registerId">ID of a register the </param>
		/// <param name="participant">JSON Participant in the Post Body</param>
		/// <param name="walletAddress">Optional override to provide a signing Wallet Address</param>
		/// <returns>Created if done</returns>
		[HttpPost("PublishParticipant/{registerId}/{walletAddress}")]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.TenantAdminRole},{Constants.BlueprintAdminRole}")]
		public async Task<ActionResult<TransactionModel>> PublishParticipant([FromRoute] string registerId, [FromRoute] string walletAddress, [FromBody] Participant participant)
		{
			// TODO SO some checking
			// if (User != null)
			//    var user = User.Identity;  // so we are currently anonymous !!
			// confirm the User has required access from the DB

			// confirm the tenant has the required register

			if (string.IsNullOrWhiteSpace(participant.Id))
				participant.Id = Guid.NewGuid().ToString();

			var tenantClaim = Request.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "tenant");
			var tenant = await _tenantRepository.Single<Tenant>(k => k.Id == tenantClaim!.Value);

			if (!tenant.Registers.Contains(registerId))
				throw new HttpStatusException(System.Net.HttpStatusCode.Forbidden, $"User Tenant is not authorized to access register: {registerId}");

			// Should probably do some kind of check to ensure the Participant Address is valid?
			TransactionMetaData meta_data = new()
			{
				RegisterId = registerId,
				TransactionType = TransactionTypes.Participant,
				TrackingData = new SortedList<string, string>
				{
					{ "organization", participant.Organisation },
					{ "name", participant.Name },
					{ "participantId", participant.Id }
				}
			};
			ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
			transaction.GetTxPayloadManager().AddPayload(JsonSerializer.SerializeToUtf8Bytes(participant));
			var tx = await _walletServiceClient.SignAndSendTransaction(transaction.GetTxTransport().transport, walletAddress);
			return Created($"{Constants.RegisterAPIURL}/{registerId}/{tx.TxId}", tx);
		}

		[HttpGet("{registerId}/participants")]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.RegisterReaderRole}")]
		public async Task<ActionResult<List<Participant>>> GetPublishedParticipants([FromRoute] string registerId)
		{
			var transactions = await _registerServiceClient.GetParticipantTransactions(registerId);
			if (transactions == null)
				return new List<Participant>();

			var payloadTasks = transactions.Select(transaction => _walletServiceClient.GetAccessiblePayloads(transaction));
			var payloads = (await Task.WhenAll(payloadTasks)).ToList();
			var participants = new List<Participant>();

			foreach (var payload in payloads)
			{
				var jsonString = Encoding.UTF8.GetString(payload[0]) ?? "{}";
				var participant = JsonSerializer.Deserialize<Participant>(jsonString);
				if (participant != null)
					participants.Add(participant);
			}
			return participants;
		}

		[HttpGet("{registerId}/participant/{participantId}")]
		[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.RegisterReaderRole}")]
		public async Task<ActionResult<Participant>> GetPublishedParticipantById([FromRoute] string registerId, [FromRoute] string participantId)
		{
			var transactions = await _registerServiceClient.GetParticipantTransactions(registerId);
			if (transactions == null)
				return NotFound(new Participant());

			TransactionModel? transactionTarget = null;
			foreach (var transaction in transactions)
			{
				if (transaction.MetaData == null)
					continue;
				if (transaction.MetaData.TrackingData.ContainsKey("participantId"))
				{
					if (transaction.MetaData.TrackingData["participantId"] == participantId)
					{
						transactionTarget = transaction;
						break;
					}
				}
			}

			if (transactionTarget == null)
				return NotFound(new Participant());
			var payloads = (await _walletServiceClient.GetAccessiblePayloads(transactionTarget)).ToList();
			if (payloads.Count != 1)
				return UnprocessableEntity("Transaction contains more then 1 payload or doesn't contain ones");

			var payload = payloads[0];
			var jsonString = Encoding.UTF8.GetString(payload) ?? "{}";
			var participant = JsonSerializer.Deserialize<Participant>(jsonString);
			if (participant == null)
				return UnprocessableEntity("Payload doesn't contain participant's information");
			return Ok(participant);
		}

		[Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[Topic("pubsub", Topics.RegisterCreatedTopicName)]
		[HttpPost("registercreated")]
		public async Task RegisterCreated([FromBody] RegisterCreated registerCreated)
		{
			var tenant = await _tenantRepository.Single<Tenant>(t => t.Id == registerCreated.TenantId);
			tenant.Registers.Add(registerCreated.Id);
			await UpdateTenant(tenant.Id!, tenant);
		}

		[Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[Topic("pubsub", Topics.RegisterDeletedTopicName)]
		[HttpPost("registerdeleted")]
		public async Task RegisterDeleted([FromBody] RegisterDeleted registerDeleted)
		{
			var tenant = await _tenantRepository.Single<Tenant>(t => t.Id == registerDeleted.TenantId);
			tenant.Registers.Remove(registerDeleted.Id);
			await UpdateTenant(tenant.Id!, tenant);
		}
	}
}
