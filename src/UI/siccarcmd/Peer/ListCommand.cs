using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;

namespace siccarcmd.peer
{
    public class ListCommand : Command
    {
        public PeerServiceClient CommsHandler = null;

        public ListCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "list";
            this.Description = "Lists the Peers of this Peer";

            CommsHandler = services.GetService<PeerServiceClient>();
            Handler = CommandHandler.Create<bool>(ListPeers);
        }

        private async Task ListPeers(bool update)
        {
            // get all info from the Peer 
            // display details on named only
            var peerInfo = await CommsHandler.GetPeersInfo();
            Console.WriteLine("Peer State : \n {0}", peerInfo.RootElement.GetRawText());
        }
    }
}
