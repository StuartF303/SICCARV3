using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Siccar.Common.ServiceClients;
using Siccar.Network.Peers.Core;

namespace siccarcmd.peer
{
    public class AddPeerCommand : Command
    {
        public PeerServiceClient peerHandler = null; 

        public AddPeerCommand(string action, IServiceProvider services) : base(action)
        {
            this.Name = "add";
            this.Description = "initiates a join network command to this peer.";

            peerHandler = services.GetService<ISiccarServiceClient>() as PeerServiceClient;
            Add(new Argument<string>("peerUri", description: "A remote peer name or address"));
            Handler = CommandHandler.Create<string, int>(AddPeer);
        }

        private void AddPeer(string peerUri, int port = 5003)
        {
            try
            {
                var newpeer = new Uri(peerUri);
                Console.WriteLine("Adding Peer : {0}", peerUri);
                JsonDocument initiatePeer = JsonSerializer.SerializeToDocument(new Peer() { URIEndPoint = newpeer });
                var result = peerHandler.RequestAddPeer(initiatePeer).Result;
                Console.WriteLine(">> {0}", result);

            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED: {0}", ex.Message);
            }
        }
    }
}
