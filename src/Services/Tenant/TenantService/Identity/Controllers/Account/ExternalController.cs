using AspNetCore.Identity.MongoDbCore.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Siccar.Common;
using Siccar.Platform.Tenants.Authorization;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using Siccar.Platform;
using Siccar.Platform.Tenants.Authentication;
using Microsoft.Extensions.Azure;
using System.Linq.Expressions;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly ILogger<ExternalController> _logger;
        private readonly IConfiguration _config;
        private readonly IAuthClaimsFactory _authClaimsFactory;
        private readonly ITenantRepository _tenantRepository;
        private readonly TenantSelectionProvider _tenantSelectionProvider;


        public ExternalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger,
            IAuthClaimsFactory authClaimsFactory, 
            IConfiguration config, 
            ITenantRepository tenantRepository,
            TenantSelectionProvider tenantSelectionProvider,
            RoleManager<ApplicationRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _logger = logger;
            _config = config;
            _authClaimsFactory = authClaimsFactory;
            _tenantRepository = tenantRepository;
            _tenantSelectionProvider = tenantSelectionProvider;
            _logger.LogDebug("External Started");
            _roleManager = roleManager;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);

        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            _logger.LogDebug("External Callback: Started");

            //Thread.Sleep(1500);
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            //  if (_logger.IsEnabled(LogLevel.Debug))
            //  {
            var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
            //  }
            string idp_sub = result.Principal.Claims.Where(s => s.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value ?? "";

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
            var tenantId = _tenantSelectionProvider.GetCurrentTenant();
            var tenant = await _tenantRepository.Single<Tenant>(tenant => tenant.Id == tenantId);
            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user

                var userCount = _userManager.Users.Count();
                _logger.LogDebug("Provisioning User on Tenant : {@tenantId} Provider/User: {@provider} / {@user} ", tenantId, provider, providerUserId);

                user = await AutoProvisionUserAsync(provider, providerUserId, claims, tenantId);
                
                // Add all roles to the first user created
                if (userCount == 0)
                {
                    _logger.LogDebug("Init First User with all roles");
                    var allRoles = _roleManager.Roles.ToList();
                    await _userManager.AddToRolesAsync(user, allRoles.Select(role => role.Name));
                }
            }
            else
            {
                _logger.LogDebug("Found User : {@user}",user.Id);
            }
            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            _logger.LogDebug("User : {@userId} \n {@user}", user.Id, user);
            string name = "";
            try
            {
                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                additionalLocalClaims.Add(new Claim("idp_sub", idp_sub));
                additionalLocalClaims.AddRange(principal.Claims);
                name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id.ToString();
            }
            catch (Exception ex) 
            {
                _logger.LogError("Error logging in : {@ermsg}", ex.Message);
            }

            var issuer = new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = name,
                IdentityProvider = tenant.Authority,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(issuer, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {

            var externalUser = result.Principal;
            _logger.LogDebug("Tenant:Finduser {0}", externalUser);

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims, string tenantId)
        {
            _logger.LogDebug("Tenant:Autoprovisioning {0}", providerUserId);

            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }

            // tenant and audience
            string audience = _config["Audience"] ?? "siccar.dev";
            filtered.AddRange(_authClaimsFactory.BuildLocalAuthorizationClaims(tenantId));

            var user = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
            };

            try
            {
                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

                if (filtered.Any())
                {
                    identityResult = await _userManager.AddClaimsAsync(user, filtered);
                    if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
                }

                identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Tenant User Creation Error", ex);
            }


            return user;
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            _logger.LogDebug("Tenant: ProcessLoginCallback");
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}