using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Siccar.Network.Peers.Core;

namespace Siccar.Network.Router
{
    /// <summary>
    /// Part of the transport is that Peers need to inform each other of updates
    /// </summary>
    public class PeerHub : Hub
    {

        private readonly List<string> groups = new List<string>() { "peers" };
        private readonly IPeerRouter _router = null;
        private readonly ILogger Log; // Not Hooked UP !!

        public PeerHub(IPeerRouter Router, ILogger<PeerHub> logger)
        {
            _router = Router;
            Log = logger;
        }

        public string HubName()
        {
            return _router.Self.Id;
        }

        public async Task DirectMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        /// <summary>
        /// We are also going to use this to update the remotePeer with last comms time / once connection broken the server will timeout! (once written) !
        /// </summary>
        /// <returns></returns>
        public Task GetConnectionId()
        {
            return Clients.Caller.SendAsync("ReceiveMessage", Context.ConnectionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Broadcast(string message)
        {
            Log.LogDebug("[P2P:SignalR] Broadcast Peer Message : {0} ", message);
            return Clients.Groups(groups).SendAsync("ReceiveMessage", Clients.Caller.ToString(), message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Update(Peer updatePeer, PeerStatus status)
        {
            Log.LogDebug("[P2P:SignalR] Peer Update : {0} ", updatePeer.PeerName);
            var myConId = Clients.Caller;


            switch (status)
            {
                case PeerStatus.Connecting:
                    // asking for a Status Update

                    _router.UpdatePeer(updatePeer);

                    break;

                case PeerStatus.Disconnecting:

                    break;

                default:

                    break;
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Called when receive and inbound transaction
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        public Task ReceiveMessage(PeerMessage tx)
        {
            Log.LogDebug("[P2P:SignalRHub] Received Message TXId : {0} TTL: {1} ", tx.TxId, tx.TTL);



            // call the appropriate Handler Instance so we can update some control data
            var caller = _router.ActivePeers.First<Peer>();  //  (_router.ActivePeers.First<Peer>(p => p.Connection.Id == Context.ConnectionId))
            caller.Connection.ReceivedMessage(tx);

            // and send it on its way

            if (tx.TTL-- >= 0)
            {
                // should be all but sender
                // return Clients.OthersInGroup("peers").SendAsync("ReceiveTransaction", tx);
                return Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveTransaction", tx);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Receive Docket
        /// </summary>
        /// <param name="Dx">An inbound Docket of Transactions</param>
        /// <returns></returns>
        public Task ReceiveDocket(PeerMessage Dx)
        {
            List<string> groups = new List<string>() { "peers" };
            return Clients.Groups(groups).SendAsync("ReceiveDocket", Dx);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "peers");
            Log.LogDebug("[P2P:SignalR] SignalR Connection {0}", Context.ConnectionId);
            var rm = new Peer
            {
                PeerName = Context.ConnectionId
            };
            //    _manager._localPeer.Peers.Add(rm);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// A Remote Peer connection has failed.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            if (exception != null)
                Log.LogWarning("[P2P][HUB] Faulted : {0}", exception.Message ?? "Remote Close");
            else
                Log.LogDebug("[P2P][HUB] Disconnect ConnectionId: {0}", Context.ConnectionId);

            _router.DisconnectPeer(Context.ConnectionId);
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "peers");
            await base.OnDisconnectedAsync(exception);
        }

    }
}
