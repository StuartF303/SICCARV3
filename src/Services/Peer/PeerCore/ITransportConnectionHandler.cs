using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Core
{
    /// <summary>
    /// The Generic abstract class for a Connection Handler
    /// </summary>

    public interface ITransportConnectionHandler : IDisposable
    {

        public string Id { get; set; }


        public abstract Task<int> QueryConnection();

        /// <summary>
        /// Opens a remote connection to a Peer
        /// </summary>
        /// <param name="remotePeer"></param>
        /// <returns></returns>
        public abstract Task OpenConnection(Peer remotePeer);


        /// <summary>
        /// Cleanly and nicely closes the connection
        /// </summary>
        /// <returns></returns>
        public abstract Task CloseConnection();


        /// <summary>
        /// Event Called when an unexpected change of status happens
        /// </summary>
        /// <returns></returns>
        public delegate PeerStatus StatusChanged();

        /// <summary>
        /// Ask the Peer to Reprobe me
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task UpdateConnection();


        /// <summary>
        /// Send a Message to a Specificed Peer
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task SendMessage(PeerMessage message);


        /// <summary>
        /// Is Called when a message is recevied from the peer
        /// </summary>
        /// <returns></returns>
        public abstract Task ReceivedMessage(PeerMessage message);


    }

}
