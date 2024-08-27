using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Siccar.Common.ServiceClients;

namespace siccarcmd.Auth
{

    /// <summary>
    /// A command to help display and debug Auth
    /// </summary>
    public class LogoutCommand : Command
    {
        public ClaimsPrincipal claimsUser = null;
        public IConfiguration configuration = null;
        private UserAuthentication userAuthentication = null;

        public LogoutCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "logout";
            this.Description = "Removes the users access to services and token cache";

            claimsUser = services.GetService<ClaimsPrincipal>();
            configuration = services.GetService<IConfiguration>();
            userAuthentication = services.GetService<UserAuthentication>();
            Handler = CommandHandler.Create(Logout);
        }

        private async Task Logout()
        {
            await Task.Run(() =>
            {
                var result = userAuthentication.LogoutCLI();
                Console.WriteLine("Logged out.");
            });
        }
    }
}
