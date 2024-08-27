using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Platform;
using Siccar.Registers.Core;
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Siccar.Common.Adaptors;
using Siccar.Common.Exceptions;
using Siccar.Registers.RegisterService.V1.Services;
using Siccar.Platform.Registers.Core;
using Siccar.Platform.Registers.Core.Models;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Registers.RegisterService.V1
{
    /// <summary>
    /// API to create, control, query and act upon Registers 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route(Constants.RegisterAPIURL)]
    [Produces("application/json")]
    [SwaggerTag("Registers are shared single truths over time", externalDocsUrl: "https://siccar.net/docs/Registers")]

    public class RegistersController : ControllerBase
    {
        /// <summary>
        /// Local Privates.
        /// </summary>
        public const string PubSub = "pubsub";
        private readonly ILogger<RegistersController> logger;
        private readonly IRegisterRepository registerRepository;
        private readonly IRegisterResolver _registerResolver;
        private readonly IDaprClientAdaptor _daprClient;
        private const int MaximumAllowedRegisters = 25;

        /// <summary>
		/// RegistersController Constructor with logger injection
		/// </summary>
		/// <param name="logger">a logger stream</param>
		/// <param name="registerRepository">a DI'd in register repository</param>
		/// <param name="registerResolver"></param>
		/// <param name="daprClient"></param>
		public RegistersController(ILogger<RegistersController> logger, IRegisterRepository registerRepository, IRegisterResolver registerResolver,
            IDaprClientAdaptor daprClient)
        {
            this.logger = logger;
            this.registerRepository = registerRepository;
            _registerResolver = registerResolver;
            _daprClient = daprClient;
        }

        /// <summary>
        /// Register - Returns a list of Registers the Authenticated user has access to. 
        /// </summary>
        /// <remarks>
        /// 
        /// Returns an Enumerable list of Registers the given Security Bearer has access to.
        /// This is ODATA V4 enabled so the following example v1/Ledger?$filter=Name eq 'BTC'
        /// 
        ///     GET ~/api/Register
        ///     
        ///     {
        ///         "@odata.context": "http://localhost:63320/api/$metadata#Registers",
        ///         "value": [
        ///            {
        ///             "id": "sample",
        ///             "name": "A Sample Register",
        ///             "height": 13,
        ///             "votes": "string",
        ///             "advertise": true,
        ///             "isFullReplica": true,
        ///             "status": "ONLINE"
        ///            },
        ///            {
        ///             "id": "secondsample",
        ///             "name": "A Second Sample Register",
        ///             "height": 5,
        ///             "votes": "string",
        ///             "advertise": true,
        ///             "isFullReplica": true,
        ///             "status": "ONLINE"
        ///            }
        ///         ]
        ///     }
        ///     
        /// </remarks>
        /// <permission  cref="string"></permission>
        /// <example>
        /// an example bit of code ...
        /// </example>
        /// <returns>A list of Registers</returns>
        /// <response code="200">Returns an enumerable of Registers</response>
        /// <response code="400">Error model</response>
        /// <response code="404">The entity could not be found</response>
        /// <response code="401">Unauthorized</response>
        [EnableQuery]
        [HttpGet("", Name = "GetRegisters", Order = 0)]
        [ProducesResponseType(typeof(IQueryable<Register>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [Authorize(Roles = $"{Constants.InstallationAdminRole},{Constants.InstallationBillingRole},{Constants.InstallationReaderRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<IActionResult> GetAllRegisters()
        {
            IEnumerable<Register> registers = new List<Register>();
            if (User.Identity?.AuthenticationType != "DaprScheme")
            {
                // we need to confirm that its a real user and return selected register 
                registers = await _registerResolver.ResolveRegistersForUser(Request.HttpContext.User.Claims);
            }
            else
            {
                //otherwise its a system request and no filtering
                registers = await registerRepository.GetRegistersAsync();
            }
            return Ok(registers);
        }

        /// <summary>
        /// Register - Returns a specified Register by Id if the Authenticated user has access to. 
        /// </summary>
        /// <remarks>
        /// 
        /// Returns an Enumerable list of Ledgers the given Security Bearer has access to.
        /// This is ODATA V4 enabled so the following example v1/Register?$filter=Name eq 'BTC'
        /// 
        ///     GET api/Registers/'000000'
        ///     
        ///     {
        ///         "@odata.context": "http://localhost/api/$metadata#Registers",
        ///         "value": [
        ///                     {
        ///                             "id": "000000",
        ///                             "ledgerType": "Undefined",
        ///                             "name": "TestRegister",
        ///                             "currentPublicKeyDepth": 0,
        ///                             "height": 2,
        ///                             "publicKey": "",
        ///                             "advertise": true,
        ///                             "fullReplica": false,
        ///                             "externalPublicKeys": []
        ///                     }
        ///                  ]
        ///     }
        /// </remarks>
        /// <permission  cref="string"></permission>
        /// <param name="RegisterId">The ID of the Register you are looking for</param>
        /// <example>
        ///     an example bit of code ...
        /// </example>
        /// <returns>A Registers by Id </returns>
        /// <response code="200">Returns an enumerable of Wallet</response>
        /// <response code="400">Error model</response>
        /// <response code="404">The entity could not be found</response>
        /// <response code="401">Unauthorized</response>
        [EnableQuery]
        [HttpGet("{RegisterId}", Name = "GetRegisterById", Order = 1)]
        [ProducesResponseType(typeof(Register), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [Authorize(Roles = $"{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<SingleResult<Register>>> GetRegisterByIdAsync([FromRoute] string RegisterId)
        {
            var result = await registerRepository.GetRegisterAsync(RegisterId);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Registers - Creates a new Register based on the supplied data, if your allowed 
        /// </summary>
        /// <remarks>Creates a new register based on format</remarks>
        /// <returns>Status code for operation</returns>
        [EnableQuery]
        [HttpPost(Name = "Post", Order = 2)]
        [ProducesResponseType(typeof(Register), Status201Created)]
        [Authorize(Roles = $"{Constants.RegisterCreatorRole}")]
        public async Task<IActionResult> PostRegister([FromBody] Register newRegister)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // probably more checking   

            // and really we want to spin up a new Collection per Register
            try
            {
                var registerCount = await registerRepository.CountRegisters();
                if (registerCount >= MaximumAllowedRegisters)
                {
                    return BadRequest($"A maximum of {MaximumAllowedRegisters} registers are allowed");
                }

                var reg = await registerRepository!.InsertRegisterAsync(newRegister);
                var tenantId = Request.HttpContext.User.FindFirst("tenant")!.Value;
                var message = new RegisterCreated
                {
                    Id = reg.Id,
                    Name = reg.Name,
                    TenantId = tenantId
                };

                await _daprClient.PublishEventAsync(RegisterConstants.PubSub, Topics.RegisterCreatedTopicName, message);
                return Created(Constants.RegisterAPIURL + "/" + newRegister.Id, reg);
            }
            catch (Exception er)
            {
                logger.LogError("Failded Post Register : {msg} ", er.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Registers - Deletes a Register based on the supplied data, if your allowed 
        /// </summary>
        /// <remarks>Creates a new register based on format</remarks>
        /// <returns>Status code for operation</returns>
        [EnableQuery]
        [HttpDelete("{registerId}", Name = "DeleteRegister", Order = 3)]
        [Authorize(Roles = $"{Constants.RegisterCreatorRole}")]
        public async Task<IActionResult> DeleteRegister(string registerId)
        {
            // Will be needing Super Admin rights to remove a local copy of a register

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // probably more checking   

            // and really we want to spin up a new Collection per Register

            var tenantId = Request.HttpContext.User.FindFirst("tenant")!.Value;
            var message = new RegisterDeleted
            {
                Id = registerId,
                TenantId = tenantId
            };

            await _daprClient.PublishEventAsync(RegisterConstants.PubSub, Topics.RegisterDeletedTopicName, message);
            await registerRepository.DeleteRegisterAsync(registerId);
            return new StatusCodeResult(200);
        }
    }
}