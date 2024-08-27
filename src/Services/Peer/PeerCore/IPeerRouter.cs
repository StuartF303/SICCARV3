using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Core
{
    /// <summary>
    /// A router manages function of connections
    /// </summary>
    public interface IPeerRouter
    {
        /// <summary>
        /// A List of current Peers
        /// </summary>
        public IEnumerable<Peer> ActivePeers { get; set; }

        /// <summary>
        /// A List of known Peers and Registers 
        /// </summary>
        public IDictionary<Peer, Register> NetworkRegisterTable { get; set; }


        /// <summary>
        /// The local Router definition
        /// </summary>
        public Peer Self { get; set; }

        /// <summary>
        /// A Method to setup the local peer
        /// </summary>
        /// <returns></returns>
        public Task InitialisePeer();

        /// <summary>
        /// A Method to find peers
        /// </summary>
        /// <returns></returns>
        public Task DiscoverPeers();


        /// <summary>
        /// A Method to refresh peers view of us
        /// </summary>
        /// <returns></returns>
        public Task UpdatePeers();

        /// <summary>
        /// A Method to refresh peers view of us
        /// </summary>
        /// <returns></returns>
        public Task UpdatePeer(Peer RemotePeer);

        /// <summary>
        /// A Methods to Query a peer
        /// </summary>
        /// <returns></returns>
        public Task<Peer> ProbePeer(Peer RemotePeer);

        public Task<Peer> ProbePeer(Uri RemotePeer);


        /// <summary>
        /// Add a Peer to the Active connecitons table
        /// </summary>
        /// <param name="RemotePeer"></param>
        /// <returns>true if added, false if fails</returns>
        public Task<bool> AddPeer(Peer RemotePeer, bool refresh = false);

        /// <summary>
        /// Remove a Peer from the Current Active Peers
        /// </summary>
        /// <param name="RemotePeer"></param>
        public void RemovePeer(Peer RemotePeer);


        /// <summary>
        /// For what ever moethod the router uses bring the connection up
        /// </summary>
        /// <param name="peer"></param>
        /// <returns>true if success</returns>
        public Task<bool> ConnectPeer(Peer peer);

        /// <summary>
        /// For the Comms hub to report a disconnection to the Router
        /// </summary>
        /// <param name="ConnectionId"></param>
        public void DisconnectPeer(string ConnectionId);

        /// <summary>
        /// Tell all my peers interesed in this Register about this message
        /// </summary>
        /// <param name="RegisterId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task BroadcastMessage(string RegisterId, PeerMessage message);

        /// <summary>
        /// call to shutdown
        /// </summary>
        public void Shutdown();
    }
}
