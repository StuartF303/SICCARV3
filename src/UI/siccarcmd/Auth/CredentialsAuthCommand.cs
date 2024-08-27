using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using siccarcmd;
using Siccar.Common;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Siccar.Common.ServiceClients;
using IdentityModel;
using IdentityModel.Client;
using System.Net.Http;

namespace siccarcmd.Auth
{

    /// <summary>
    /// A command to help display and debug Auth
    /// </summary>
    public class CredentialsAuthCommand : Command
    {
        public ClaimsPrincipal claimsUser = null;
        public IConfiguration configuration = null;
        private UserAuthentication userAuthentication = null;
        private DiscoveryDocumentResponse disco = null;
        private HttpClient client = null;

        public CredentialsAuthCommand(string action, IServiceProvider services) : base(action)
        {

            // Initialise Commands
            this.Name = "credentials";
            this.Description = "Gains machine access to services via client credential flow..";

            claimsUser = services.GetService<ClaimsPrincipal>();
            configuration = services.GetService<IConfiguration>();
            userAuthentication = services.GetService<UserAuthentication>();

            string defaultClient = string.IsNullOrWhiteSpace(configuration["clientId"]) ? "siccar-admin-ui-client" : configuration["clientId"];
            string defaultSecret = string.IsNullOrWhiteSpace(configuration["clientSecret"]) ? string.Empty : configuration["clientSecret"];
            string defaultRoles = string.IsNullOrWhiteSpace(configuration["Scope"]) ? "wallet.user" : configuration["Scope"];

            this.Add(new Option<string>(new string[] { "--clientid", "-c" }, getDefaultValue: () => defaultClient, description: "API Client Id"));
            this.Add(new Option<string>(new string[] { "--secret", "-s" }, getDefaultValue: () => defaultSecret, description: "API Client Secret"));
            this.Add(new Option<string>(new string[] { "--roles", "-r" }, getDefaultValue: () => defaultRoles, description: "API Role Scopes"));

            Handler = CommandHandler.Create<string, string, string>(Login);
        }

        private async Task Login(string clientId, string secret, string roles)
        {

            client = new HttpClient();

            disco = await client.GetDiscoveryDocumentAsync(configuration["SiccarService"]);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            var conUrl = disco.TokenEndpoint ?? "connect/token";

            // to be extra sure you can ask for openid or profile so going to remove them
            if (roles.Contains("openid"))
                Console.WriteLine("Scope contains openid which is not valid for Client Creds");

            var result = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = conUrl,
                ClientId = clientId,
                ClientSecret = secret,
                Scope = roles
            });

            if (!result.IsError)
            {
                Console.WriteLine($"user code   : {result.AccessToken}");
            }
            else
                Console.WriteLine($"Failed : {result.Error}");

        }

        private async Task<TokenResponse> RequestTokenAsync(DeviceAuthorizationResponse authorizeResponse)
        {
            Console.WriteLine($"Getting Access Token: ...");
            while (true)
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "cli",
                    DeviceCode = authorizeResponse.DeviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        await Task.Delay(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }
    }
}
