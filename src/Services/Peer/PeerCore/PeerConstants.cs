using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Core
{

    public static class PeerConstants
    {
        public const int DefaultConnections = 25;
        public const int DefaultPingWait = 30000;

        public const string PeerHubUri = "PeerHub";

        public const string PubSubName = "pubsub";
        public const string RxTopicName = "OnReceivedTransactions";
    }
}
