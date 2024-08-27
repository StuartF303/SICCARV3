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
using IdentityModel;
using System.Security.Claims;
using Siccar.Common.ServiceClients;

namespace siccarcmd.Auth
{

    /// <summary>
    /// A command to help display and debug Auth
    /// </summary>
    public class InfoCommand : Command
    {
        public ClaimsPrincipal claimsUser = null;
        public IConfiguration configuration = null;
        private UserAuthentication userAuthentication = null;

        public InfoCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = action;
            this.Description = "Displays information about the current users authentication";

            claimsUser = services.GetService<ClaimsPrincipal>();
            userAuthentication = services.GetService<UserAuthentication>();
            configuration = services.GetService<IConfiguration>();

            Handler = CommandHandler.Create(DisplayStatus);
        }

        private async Task DisplayStatus()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (userAuthentication.AuthCache.IsLoaded)
                    {
                        Console.WriteLine("Authentication : \n");
                        //                  Console.WriteLine("\t Authority : {0}", app.Authority);
                        //                  Console.WriteLine("\t User : {0}", accounts.FirstOrDefault().Username);
                        Console.WriteLine("\t Claims : ");
                        foreach (var t in claimsUser.Claims)
                        {
                            Console.WriteLine("\t\t Claim : {0}  =  {1}", t.Type, t.Value);
                        }

                        Console.WriteLine("\t Tokens :  ");
                        Console.WriteLine("\t\t Identity : {0} \n", userAuthentication.AuthCache.IdentityToken);
                        Console.WriteLine("\t\t Access   : {0} \n", userAuthentication.AuthCache.AccessToken);
                        Console.WriteLine("\t\t Refresh  : {0} \n", userAuthentication.AuthCache.RefreshToken);
                    }
                    else
                    {
                        Console.WriteLine("No loaded user context.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
