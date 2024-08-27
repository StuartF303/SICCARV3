using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using Siccar.Application;
using Siccar.Common;
using Siccar.Common.ServiceClients;
using Siccar.Platform.Tenants.Repository;
using Siccar.Platform.Tenants.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace Siccar.Platform.Tenants.V1.Controllers
{
    [Route("api/init")]
    [ApiController]
    [AllowAnonymous]
    public class InitController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<TenantsController> _logger;
        private JsonSerializerOptions jopts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        private IEnumerable<Tenant> _tenants = new List<Tenant>();
        private IEnumerable<Client> _clients = new List<Client>();

        public InitController(ILogger<TenantsController> logger, ITenantRepository tentantRepository)
        {
            _logger = logger;
            _tenantRepository = tentantRepository;
        }

        /// <summary>
        /// Self contained bootstrap controller
        /// </summary>
        /// <param name="bootInfo"></param>
        /// <returns></returns>

        [HttpPost()]
        public async Task<ActionResult> SetConfig(JsonDocument bootInfo)
        {
            string newTenantId = string.Empty;
            // first check we dont already have a config 
            if (_tenantRepository.CollectionExists<Tenant>())
                return NotFound();

            // Parse the bootInfo
            try
            {
                var propTnt = bootInfo.RootElement.GetProperty("tenants").GetRawText();
                _tenants = JsonSerializer.Deserialize<List<Tenant>>(propTnt, jopts) ?? new List<Tenant>();

                var propCl = bootInfo.RootElement.GetProperty("clients");
                _clients = JsonSerializer.Deserialize<List<Client>>(propCl, jopts) ?? new List<Client>();
            }
            catch (KeyNotFoundException kerr)
            {
                _logger.LogDebug($"Data Error : {kerr.Message}");
                return BadRequest(kerr.Message);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error processing InitFile : {err.Message}");
                return BadRequest(err.Message);
            }

            try
            {
                newTenantId = _tenants.First().Id;
                await _tenantRepository.Add<Tenant>(_tenants.First());


                foreach (var client in _clients)
                {
                    client.TenantId = newTenantId;
                    await _tenantRepository.Add<Client>(client);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error commiting to repository : {err.Message}");
                return BadRequest(err.Message);
            }

            return Ok(newTenantId);
        }
    }
}
