using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;

using System.Text.Json;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Microsoft.Extensions.Configuration;

namespace siccarcmd.Config
{
    /// <summary>
    /// Register Create Command
    /// </summary>
    public class ListCommand : Command
    {
        private readonly IConfiguration configuration = null;
        /// <summary>
        /// List configuration Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public ListCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            Name = "list";
            Description = "Writes out our current running local configuration";
            configuration = services.GetService<IConfiguration>();
            Handler = CommandHandler.Create(ListSettings);
        }

        /// <summary>
        /// List config Command Handler
        /// </summary>

        private async Task ListSettings()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("****************************************************************");
                Console.WriteLine("* SICCAR Command Line Admin Client Configuration               *");
                Console.WriteLine("**************************************************************** \n");
                Console.WriteLine($"\t Service Host : {configuration["SiccarService"]}");
                Console.WriteLine("\n\n");
            });
        }
    }
}
