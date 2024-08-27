using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace siccarcmd.Config
{
    /// <summary>
    /// Settings Commadn - Changes local settings
    /// changes are stored in ~/.siccar/appsettings.json
    /// </summary>
    public class SetCommand : Command
    {
        /// <summary>
        /// Set Command interface parser
        /// </summary>
        /// <param name="action"></param>
        public SetCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            Name = "set";
            Description = "Modifies local configurations";
            Add(new Option<string>(new string[] { "-h", "--host" }, () => "https://localhost:8443/", description: "Will set the Register to broadcast its presence."));
            Handler = CommandHandler.Create<string>(SetConfiguration);
        }

        /// <summary>
        /// Set Configuration Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async Task SetConfiguration(string name)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"Change the file : ~/.siccar/appsettings.json");
            });
        }
    }
}
