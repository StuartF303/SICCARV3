using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
#nullable enable

namespace siccarcmd.register
{
    /// <summary>
    /// Register Create Command
    /// </summary>
    public class TransactionCommand : Command
    {
        public readonly IRegisterServiceClient? commshandler = null;

        /// <summary>
        /// Create Register Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public TransactionCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "transaction";
            this.Description = "Performs get actions on a Register Transactions";

            Add(new Argument<string>("name", description: "A friendly name for the Register."));
            Add(new Argument<string>("txid", description: "A register transaction Id"));

            // Add(new Option<string>(new string[] { "-q", "--query" }, description: "Specify a query"));

            commshandler = services.GetService<IRegisterServiceClient>();
            Handler = CommandHandler.Create<string, string, string>(GetTransactions);
        }

        /// <summary>
        /// Register Create Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async void GetTransactions(string name, string txid, string? query = null)
        {
            if (commshandler != null)
            {
                var ret = await commshandler.GetTransactionById(name, txid);
                Console.WriteLine("Details of Transaction : {0} [{1}]", name, txid);
                Console.WriteLine(ret);
            }
        }
    }
}
