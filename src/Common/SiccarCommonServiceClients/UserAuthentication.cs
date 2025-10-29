// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using IdentityModel.OidcClient;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Siccar.Common.ServiceClients
{
    public class UserAuthentication
    {

        private readonly IConfiguration configuration;
        private readonly string _authority = "http://localhost";
        private OidcClient? _oidcClient = null;
        private OidcClientOptions? _oidcConfiguration = null;

        public ClaimsPrincipal principal { get; set; } = new ClaimsPrincipal();

        public AuthCache? AuthCache = null;

        public UserAuthentication(IConfiguration Configuration)
        {
            configuration = Configuration;
            _authority = string.IsNullOrWhiteSpace(Configuration["SiccarService"]) ? "http://localhost" : Configuration["SiccarService"]!;
            AuthCache = new AuthCache();
        }

        public async Task<string> LoginCLI(string scope = "openid profile wallet.user", string clientId = "siccar-admin-ui-client", string secret = "")
        {

            var browser = new SystemBrowser(6004); // this could be taken from configuration

            string redirectUri = string.Format($"http://localhost:{browser.Port}");

            _oidcConfiguration = new OidcClientOptions()
            {
                Authority = _authority,
                ClientId = clientId,
                RedirectUri = redirectUri,
                Scope = scope,
                FilterClaims = false,
                Browser = browser
            };

            if (!string.IsNullOrEmpty(secret))
            {
                _oidcConfiguration.ClientSecret = secret;
            }

            _oidcClient = new OidcClient(_oidcConfiguration);
            var result = await _oidcClient.LoginAsync();

            if (result.User != null)
                if (result.User.Identity is not null)
                {
                    AuthCache!.AccessToken = result.AccessToken;
                    AuthCache.IdentityToken = result.IdentityToken;
                    AuthCache.RefreshToken = result.RefreshToken;

                    await AuthCache.Write();

                    principal = ValidateToken(AuthCache.AccessToken);
                }

            return result.AccessToken;
        }

        public async Task LogoutCLI()
        {
            AuthCache!.AccessToken = "";
            AuthCache.IdentityToken = "";
            AuthCache.RefreshToken = "";

            // should do a forced logout via IDP
            await AuthCache.Write();

        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            TokenValidationParameters validationParameters = new()
            {
                ValidateLifetime = true,

                //validationParameters.ValidAudience = _audience.ToLower();
                ValidIssuer = configuration["SiccarService"]?.ToLower()
            };
            //validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

            //ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);


            return principal;
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"\naccess token:   {result.AccessToken}");
            Console.WriteLine($"\nrefresh token:  {result?.RefreshToken ?? "none"}");
        }
    }
}
