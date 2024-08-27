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
using System.Threading;
using static Siccar.Common.ServiceClients.IActionServiceClient;

namespace siccarcmd.Action
{
    /// <summary>
    /// A CLI Command to wait for and display Action event notifications
    /// </summary>
    public  class ActionEventCommand : Command
    {
        public IActionServiceClient commshandler = null;

        /// <summary>
        /// Create Register Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public ActionEventCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "event";
            this.Description = "Watches for ActionService Events";

            Add(new Argument<string>("address", description: "A Wallet which the user has access to."));
            Add(new Option<string>(new string[] { "-b", "--bearer" }, description: "A JWT Bearer to use for AuthN."));

            commshandler = services.GetService<IActionServiceClient>();

            Handler = CommandHandler.Create<string, string>(WatchAction);
        }

        /// <summary>
        /// Register Create Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async Task WatchAction(string address, string bearer)
        {
            // nasty work around
            await commshandler.SetBearer(bearer);
            await commshandler.StartEvents();

            commshandler.OnConfirmed += ProcessEvent;

            await commshandler.SubscribeWallet(address);


            Console.WriteLine($"Connected to Hub Id: {commshandler.ConnectionId}");
             
            Console.WriteLine("Awaiting Action events for Wallet Address : {0}", address);

         
            while (true) {
                Thread.Sleep(100);
            }
        }

        public async Task ProcessEvent(TransactionConfirmed txData)
        {
            await Task.Run(() => Console.WriteLine($"Received Tx: {txData.TransactionId}"));
        }
    }
}
