using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Siccar.Common;
using Swashbuckle.AspNetCore.Annotations;
using TenantRepository;
using Asp.Versioning;

namespace Siccar.Platform.Tenants.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/users")]
    [Produces("application/json")]
    [SwaggerTag("The Users Service handles organisational user management", externalDocsUrl: "https://docs.siccar.dev/#d0ecd7d0-d66a-41f9-954c-56ac964f0860")]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.TenantAdminRole},{Constants.TenantBillingRole},{Constants.BlueprintAdminRole}")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get a user by id 
        /// </summary>
        /// <returns>Status code for operation and specified user</returns>
        [HttpGet("{id}", Name = "GetUser", Order = 1)]
        [EnableQuery(MaxTop = 100, PageSize = 10, MaxExpansionDepth = 50)]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            try
            {
                var user = await _userRepository.Get(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get the user by id {0}", id);
                throw;
            }
        }
        
        /// <summary>
        /// Get a user by id 
        /// </summary>
        /// <returns>Status code for operation and specified user</returns>
        [HttpPut("{id}", Name = "UpdateUser", Order = 1)]
        [EnableQuery(MaxTop = 100, PageSize = 10, MaxExpansionDepth = 50)]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = Constants.TenantAdminRole)]
        public async Task<ActionResult<User>> Update(Guid id, User user)
        {
            try
            {
                user.Id = id;
                var existingUser = await _userRepository.Get(id);
                if (existingUser == null)
                {
                    return NotFound("A user with the id does not exist");
                }
                user = await _userRepository.Update(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update the user {0}", user);
                throw;
            }
        }

        /// <summary>
        /// Delete a user by id 
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteUser", Order = 1)]
        [EnableQuery(MaxTop = 100, PageSize = 10, MaxExpansionDepth = 50)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var user = await Get(id);

                if (user.Result is NotFoundResult notFound)
                {
                    return notFound;
                }
                await _userRepository.Delete(id);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete the user by id {0}", id);
                throw;
            }
        }

        /// <summary>
        /// Get the users associated with the current user's tenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var tenant = HttpContext.User.FindFirst("tenant");
            var users = await _userRepository.ListByTenant(tenant!.Value);
            return Ok(users);
        }
    }
}