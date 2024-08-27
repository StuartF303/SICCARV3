using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;

namespace siccarcmd.peer
{
    /// <summary>
    /// A set of commands that display the status and details of a given peer
    /// can be put in a real-time mode that pumps updates as they happen
    /// </summary>
    public class InfoCommands : Command
    {
        public PeerServiceClient CommsHandler = null;

        public InfoCommands(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "info";
            this.Description = "Displays information about the Peer Service";

            CommsHandler = services.GetService<PeerServiceClient>();
            Handler = CommandHandler.Create<bool>(GetInfo);
        }

        private async Task GetInfo(bool update)
        {
            // get all info from the Peer 
            // display details on named only
            var peerInfo = await CommsHandler.GetPeerInfo();
            Console.WriteLine("Peer State : \n {0}", peerInfo.RootElement.GetRawText());
        }
    }
}
