using Dapr.Client;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Network.Peers.Core;
using Siccar.Platform;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Siccar.Network.Router.ConnectionHandlers
{
    public class TransportHandlerBase : ITransportConnectionHandler

    {
        public string Id { get; set; }

        public IPeerRouter router = null;

        public ILogger Log = null;

        public Peer RemotePeer = null;

        private readonly DaprClient _daprClient = null;

        public TransportHandlerBase(IPeerRouter route, DaprClient daprClient, ILogger log)
        {
            router = route;
            Log = log;

            _daprClient = daprClient;

        }

        public async Task CloseConnection()
        {
            await Task.Run(() => Log.LogInformation("[P2P:Transport] Closed connection"));
            router.RemovePeer(RemotePeer);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            router = null;
        }

        public virtual async Task OpenConnection(Peer remotePeer)
        {
            await Task.Run(() => Log.LogInformation("[P2P:Transport] Connection Opened to {name}", remotePeer.PeerName));
        }


        public Task<int> QueryConnection()
        {
            Log.LogInformation("[P2P:Transport] Connection Query");

            return Task.FromResult(0);
        }

        public virtual async Task UpdateConnection()
        {
            await Task.Run(() => Log.LogInformation("[P2P:Transport] Send Request Update"));
        }

        public virtual async Task SendMessage(PeerMessage message)
        {
            await Task.Run(() => Log.LogInformation("[P2P:Transport] Send Message ID {id}", message.TxId));
        }


        public async Task ReceivedMessage(PeerMessage message)
        {
            Log.LogInformation("[P2P:Transport] Received Message ID {id} : TTL {ttl}", message.TxId, message.TTL);

            var tx = JsonSerializer.Deserialize<TransactionModel>(message.TxBody);
            // push to Service Bus for RegisterServices
            try
            { // TODO This is currently set to save straight away we want it to be processed by validator
                await _daprClient.PublishEventAsync<TransactionModel>(PeerConstants.PubSubName, Topics.TransactionPendingTopicName , tx); 
            }
            catch(Exception er)
            {
                Log.LogCritical("DAPR FAILURE : {msg}", er.Message);
            }

            // if we are forwarding transactions this is where we do it... 
            // if (message.ttl-- > 0) broadcast(message)

        }


    }
}
