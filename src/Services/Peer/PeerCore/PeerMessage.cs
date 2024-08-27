using Siccar.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Core
{
    public class PeerMessage
    {

        /// <summary>
        /// Time to live, dies at zero 
        /// </summary>
        public int TTL { get; set; } = Constants.NetworkTimeToLive;



        public string TxId { get; set; } = "";

        public string TxBody { get; set; } = "";

    }
}
