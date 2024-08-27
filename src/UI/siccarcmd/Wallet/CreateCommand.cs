using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
#nullable enable

namespace siccarcmd.Wallet
{
    /// <summary>
    /// Register Create Command
    /// </summary>
    public class CreateCommand : Command
    {
        public WalletServiceClient? walletServiceClient = null;

        /// <summary>
        /// Create Register Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public CreateCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "create";
            this.Description = "Creates a new Wallet";

            Add(new Argument<string>("name", description: "A friendly name for the Wallet."));
            Add(new Option<string>(new string[] { "-i", "--id" }, description: "A specific Id for the Register, otherwise auto generated."));

            walletServiceClient = services.GetService<WalletServiceClient>();

            Handler = CommandHandler.Create<string, string, bool, bool>(CreateWallet);
        }

        /// <summary>
        /// Register Create Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async void CreateWallet(string name, string id, bool advertise, bool full)
        {
            await Task.Run(() => { }); 
        }
    }
}
