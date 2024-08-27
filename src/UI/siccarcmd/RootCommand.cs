using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace siccarcmd
{
    public class RootCommand : Command
    {
        public RootCommand(string action, string description = null) : base(action, description)
        {
            // Add( new Option<string>( new string[] { "bearer", "-b" }, description: "A Siccar Access JWT Token")) ;
            //configuration = services.GetService<IConfiguration>();

            Handler = CommandHandler.Create<bool, string, int, string, bool>(ProcessGlobals);
        }


        private async Task ProcessGlobals(bool debug, string uri, int port, string bearer, bool config)
        {
            await Task.Run(() =>
            {
                // Check see if we are logged in , if not do something about it
                //   var authCode = await _userAuthentication.LoginCLI();
                //   await _userAuthentication.GetAccessToken(authCode);
                Console.WriteLine("Siccar Command Tool \n siccarcmd --help \n ");

                if (debug)
                    Console.WriteLine("DEBUG: On");


                if (!string.IsNullOrEmpty(uri))
                    Console.WriteLine($"We cannot currently set the URI to : {uri}");

                if (config)
                { // lets write out our configuration


                }
            });
        }
    }
}
