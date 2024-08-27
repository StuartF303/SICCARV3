using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siccar.Platform.Tenants.Repository;
using System.Net;
using System.Threading.Tasks;
using Siccar.Common.Exceptions;
using Siccar.Common;
using IdentityServer4.Models;
using TenantService.Validation;
using Client = Siccar.Platform.Tenants.Core.Client;
using System.Collections.Generic;

namespace TenantService.V1.Controllers
{
    [Route("api/tenants/{tenantId}/clients")]
    [ApiController]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public class ClientsController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;
        public ClientsController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Gets a client by id
        /// </summary>
        [HttpGet("{clientId}", Name = "GetClientById", Order = 2)]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole},{Constants.TenantBillingRole}")]
        public async Task<ActionResult<Client>> GetClient(string tenantId, string clientId)
        {
            var client =
                await _tenantRepository.Single<Client>(client =>
                    client.TenantId == tenantId && client.ClientId == clientId);

            return Ok(client);
        }

        /// <summary>
        /// Gets all Clients For a Tenant
        /// </summary>
        /// <returns>Status code for operation and specified tenant</returns>
        [HttpGet(Name = "GetClientsByTenantId", Order = 2)]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole},{Constants.TenantBillingRole}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients(string tenantId)
        {
            var clientList = await _tenantRepository.Where<Client>(k => k.TenantId == tenantId);
            return Ok(clientList);
        }

        /// <summary>
        /// Clients - Updates a Client based on the supplied data
        /// </summary>
        /// <returns>Status code for operation and updated Client</returns>
        [HttpPut("{clientId}")]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationReaderRole},{Constants.TenantAdminRole}")]
        public async Task<IActionResult> UpdateClient(string clientId, [FromBody] Client client)
        {
            if (clientId != client.ClientId) throw new HttpStatusException(HttpStatusCode.BadRequest, "client id in url does not match client id in the body.");
            await _tenantRepository.UpdateClient(client);
            return Ok(client);
        }

        /// <summary>
        /// Allow and Admin to create a Device/Client
        /// </summary>
        /// <param name="tenantId">ID of the Users Organisational Tenant</param>
        /// <param name="client"></param>
        /// <returns>Created if done</returns>
        [HttpPost]
        [Authorize(Roles = $"{Constants.TenantAdminRole}")]
        public async Task<IActionResult> CreateClient([FromRoute] string tenantId, [FromBody] Client client)
        {
            client.TenantId = tenantId;

            foreach (var clientSecret in client.ClientSecrets)
            {
                clientSecret.Value = clientSecret.Value.Sha256();
            }

            var validator = new CreateClientValidator(_tenantRepository);
            var result = await validator.ValidateAsync(client);

            if (!result.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(result.ToDictionary()));
            }

            await _tenantRepository.Add(client);
            return Created($"api/tenants/{{tenantId}}/clients/{client.ClientId}", client);
        }

        /// <summary>
        /// Deletes a Client based on the supplied data
        /// </summary>
        /// <returns>Status code for operation</returns>
        [HttpDelete("{clientId}")]
        [Authorize(Roles = $"{Constants.TenantAdminRole}")]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            await _tenantRepository.Delete<Client>(t => t.ClientId == clientId);
            return Ok();
        }

    }
}
