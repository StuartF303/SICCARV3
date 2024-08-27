using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using Microsoft.Extensions.Configuration;
using AspNetCore.Identity.MongoDbCore.Models;
using IdentityModel;
using System;

namespace Siccar.Platform.Tenants.Authorization
{
    public class TenantProfileService : IProfileService
    {
        private readonly ILogger<TenantProfileService> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITenantRepository _tenantRepository;

        public TenantProfileService(
            ILogger<TenantProfileService> logger,
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signInManager,
            ITenantRepository tenantRepo,
            IConfiguration config
            )
        {
            _logger = logger;
            _users = users;
            _signInManager = signInManager;
            _tenantRepository = tenantRepo;
            _config = config;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _users.FindByIdAsync(context.Subject.GetSubjectId());
            _logger.LogInformation("Processing Profile Data for: {@profileId}", user.Id);


            string aud = string.IsNullOrWhiteSpace(_config["Audience"]) ? "siccar.dev" : _config["Audience"];

            string idp_sub = context.Subject.Claims.FirstOrDefault(claim => claim.Type == "idp_sub")?.Value ?? "";
            var claims = user.Claims.Where(claim => claim.Type == "tenant").ToList();

            _logger.LogDebug("Profile {@userId} has {@claimCount} claims", user.Id, claims.Count);
            if (claims.Any())
            {
                _logger.LogDebug($"Processing claims for Tenant : {claims[0].Value} of {claims.Count}");
                var tenant = await _tenantRepository.Single<Tenant>(tenant => tenant.Id == claims[0].Value);

                if (tenant != null)
                    foreach (var register in tenant.Registers)
                    {
                        claims.Add(new MongoClaim { Type = "registers", Value = register, Issuer = claims[0].Issuer });
                    }
                claims.Add(new MongoClaim { Type = "aud", Value = aud, Issuer = claims[0].Issuer });

                var roles = await _users.GetRolesAsync(user);
                _logger.LogDebug("User Roles Count {@rolesCount} \n {@roles}", roles.Count(), roles);

                // var roleClaims = roles.Select(role => new MongoClaim { Type = JwtClaimTypes.Role, Value = role, Issuer = claims[0].Issuer });
                foreach (var role in roles)
                {
                    _logger.LogDebug("Adding Role: {@role}", role);

                    MongoClaim roleClaim = new MongoClaim { Type = JwtClaimTypes.Role, Value = role, Issuer = claims[0].Issuer };

                    claims.Add(roleClaim);
                }
                // adding individually to check for nulls ... claims.AddRange(roleClaims);
                claims.Add(new MongoClaim { Type = "idp_sub", Value = idp_sub, Issuer = claims[0].Issuer });
            }
            _logger.LogDebug("Getting System Claims : {@claims}", claims);

            var systemClaims = claims.Select(claim => new Claim(claim.Type, claim.Value, string.Empty, claim.Issuer));
            if (systemClaims != null)
            {
                _logger.LogDebug("System Claim Count {@claimCount} \n {@systemClaims}", systemClaims.Count(), systemClaims);
            }
            else
            {
                _logger.LogError("System Claims are empty");
            }
            context.IssuedClaims.AddRange(systemClaims);

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogInformation("IsActive called from: {caller}", context.Caller);

            // AuthenticationType will be one of 'IdentityServer4', 'tokenvalidator'  
            // we can only use signingmanager when its a user based token
            if (context.Subject.Identity.AuthenticationType != "tokenvalidator")  // so dont do this for client creds
            {
                var subjectId = context.Subject.GetSubjectId();
                _logger.LogDebug("Finding Subject : {@subjectId}", subjectId);
                var user = await _users.FindByIdAsync(subjectId);

                if (user != null)
                {
                    _logger.LogDebug("Profile User : {@userId} \n {@user}", user.Id, user);
                    try
                    {
                        //var principal = await _signInManager.CreateUserPrincipalAsync(user);

                        //context.IsActive = _signInManager.IsSignedIn(principal);
                    }
                    catch (Exception err)
                    {
                        _logger.LogError("ERROR: Failed to SignIn @{errorMsg}", err.Message);
                    }
                }
            }
        }
    }
}
