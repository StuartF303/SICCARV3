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

namespace siccarcmd.Auth
{

    /// <summary>
    /// A command to help display and debug Auth
    /// </summary>
    public class LoginCommand : Command
    {
        public ClaimsPrincipal claimsUser = null;
        public IConfiguration configuration = null;
        private UserAuthentication userAuthentication = null;

        public LoginCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "login";
            this.Description = "Gains user access to services via authorization code flow.";

            claimsUser = services.GetService<ClaimsPrincipal>();
            configuration = services.GetService<IConfiguration>();
            userAuthentication = services.GetService<UserAuthentication>();

            string defaultClient = string.IsNullOrWhiteSpace(configuration["clientId"]) ? "siccar-admin-ui-client" : configuration["clientId"];
            string defaultSecret = string.IsNullOrWhiteSpace(configuration["clientSecret"]) ? string.Empty : configuration["clientSecret"];
            string defaultRoles = string.IsNullOrWhiteSpace(configuration["Scope"]) ? "openid installation.admin tenant.admin register.creator wallet.user" : configuration["Scope"];

            this.Add(new Option<string>(new string[] { "--clientid", "-c" }, getDefaultValue: () => defaultClient, description: "API Client Id"));
            this.Add(new Option<string>(new string[] { "--secret", "-s" }, getDefaultValue: () => defaultSecret, description: "Client Secret"));
            this.Add(new Option<string>(new string[] { "--roles", "-r" }, getDefaultValue: () => defaultRoles, description: "Role Scopes"));

            Handler = CommandHandler.Create<string, string, string>(Login);
        }

        private async Task Login(string clientid, string secret, string roles)
        {
            var result = await userAuthentication.LoginCLI(roles, clientid, secret);

            Console.WriteLine("Logged in, access cached");

        }
    }
}
