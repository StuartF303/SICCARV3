using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using System.Collections;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;
using DnsClient.Internal;

namespace TenantService.Authorization
{
    /// <summary>
    /// Intercept token generation to ensure we have the system claims in place.
    /// </summary>
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<CustomTokenRequestValidator> _logger;

        public CustomTokenRequestValidator(ITenantRepository tenantRepository, ILogger<CustomTokenRequestValidator> logger)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
        }

        /// <summary>
        /// Make sure we have the 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            if (context.Result.ValidatedRequest.GrantType != "authorization_code")
            {
               
                var client = context.Result.ValidatedRequest.Client;
                var siccarClient =
                    await _tenantRepository.Single<Client>(siccarClient =>
                        client.ClientId == siccarClient.ClientId);

                _logger.LogDebug($"Processing claims for Tenant : {siccarClient.TenantId}");

                Claim currentTenant = context.Result.ValidatedRequest.ClientClaims.Where(x => x.Type == "tenant").FirstOrDefault();
                if (currentTenant != null)
                    context.Result.ValidatedRequest.ClientClaims.Remove(currentTenant);

                Claim currentSub = context.Result.ValidatedRequest.ClientClaims.Where(x => x.Type == "sub").FirstOrDefault();
                if (currentSub != null)
                    context.Result.ValidatedRequest.ClientClaims.Remove(currentSub);

                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("tenant", siccarClient.TenantId));
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("sub", siccarClient.ClientId));

                
            }
        }
    }
}
