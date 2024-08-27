using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Net;
using Json.More;
using Siccar.Common;
// We are using the Macross.Json.Extension to cover some serialization issues that will hopefully be fixed in System.Text.Json .Net6 !!!

namespace Siccar.Network.Peers.Core
{
    /// <summary>
    /// The Peer is a representation of a Siccar Network connections
    /// - it has state
    /// - it is operational 
    /// - it works in realtime
    /// 
    /// </summary>
    public class Peer : IDisposable
    {
        /// <summary>
        /// A Unique ID for the Peer on the Network - this may be transitory 
        /// </summary>
        [Key]
        [DataMember]
        [Required]
        [MaxLength(38)]
        public string Id
        {
            get { return string.IsNullOrEmpty(_cachedId) ? makeId() : _cachedId; }
            set { _cachedId = value; }
        }
        private string _cachedId = "";

        /// <summary>
        /// A Friendly Name for the Remote Peer
        /// </summary>
        [DataMember]
        [Required]
        public string PeerName { get; set; } = "localhost";

        /// <summary>
        /// Connection Endpoint from a remote Peer - This is our prefence for connectivity
        /// </summary>
        [DataMember]
        [Required]
        public Uri URIEndPoint { get; set; } = new Uri("http://localhost");

        /// <summary>
        /// Connection Endpoint from a remote Peer - a fall back IP system for local or non dns networks
        /// Currently having issues with Deserializing when posted in to a controller and the default below is set... 
        /// </summary>
        [JsonConverter(typeof(JsonIPAddressConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IPAddress IPEndPoint { get; set; }  // if set fails controller post !! = IPEndPoint.Parse("127.0.0.1:443");

        /// <summary>
        /// Connection Endpoint from a remote Peer - a fall back IP system for local or non dns networks
        /// Currently having issues with Deserializing when posted in to a controller and the default below is set... 
        /// </summary>
        public int IPSocket { get; set; } = 443; // if set fails controller post !! = IPEndPoint.Parse("127.0.0.1:443");


        /// <summary>
        /// A forced lack of SSL, should not be set to true 
        /// </summary>
        [DataMember]
        public bool DisabledSSL { get; set; } = false;

        /// <summary>
        /// A list of registers supported by that Peer
        /// </summary>
        public IEnumerable<string> Registers { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public PeerStatus Status { get; set; } = PeerStatus.NotConnected;

        /// <summary>
        /// Connection Time 
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public TimeSpan ConnectedTime { get; set; } = new TimeSpan(0L);

        /// <summary>
        /// Last Connection Time 
        /// </summary>
        [DataMember]
        public DateTime LastConnectedTime { get; set; } = DateTime.MinValue;


        /// <summary>
        /// How many Connections can this Peer handle
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public int MaxConnections { get; set; } = PeerConstants.DefaultConnections;

        /// <summary>
        /// Has this Peer been add to the allow peers 
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public bool Whitelisted { get; set; } = false;

        /// <summary>
        /// A Workign set of Whitelisted peers - NO STATE STORED OR SHARED
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IEnumerable<Peer> Whitelist { get; set; } = new List<Peer>();


        /// <summary>
        /// Is this Peer restricted fromcommunicating
        /// </summary>
        [DataMember]
        public bool Blacklisted { get; set; } = false;

        /// <summary>
        /// A Working set of Blacklisted peers - NO STATE STORED OR SHARED
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IEnumerable<Peer> Blacklist { get; set; } = new List<Peer>();


        /// //////////////////////////////////////////////////
        // Ping & instrumentation


        [IgnoreDataMember]
        [JsonIgnore]
        public double PingTime { get; set; } = 0L;

        /// <summary>
        /// How long to wait between pings
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public double PingWait { get; set; } = PeerConstants.DefaultPingWait;


        [IgnoreDataMember]
        public double MaxPing { get; set; } = 0;

        [DataMember]
        public DateTime LastSend { get; set; } = DateTime.MinValue;

        [IgnoreDataMember]

        public DateTime LastRecv { get; set; } = DateTime.MinValue;


        [DataMember]
        public string Version { get; set; } = Constants.NetworkVersion;

        // The RemotePeer can read data and SignalR but not be directly pinged

        [IgnoreDataMember]
        public bool ReadOnly { get; set; } = false;


        [IgnoreDataMember]
        public int Errors { get; set; } = 0;


        // Methods 
        [IgnoreDataMember]
        [JsonIgnore]
        public ITransportConnectionHandler Connection { get; set; }


        /// <summary>
        //
        /// </summary>
        /// <returns></returns>

        private string makeId()
        {
            var src = this.URIEndPoint.Host.ToLower() + ":" + this.URIEndPoint.Port.ToString();
            _cachedId = String.Format("{0:X}", src.GetHashCode());
            return _cachedId;
        }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.CloseConnection();
                this.Connection.Dispose();
            }
        }
    }

    [JsonConverter(typeof(EnumStringConverter<PeerStatus>))]
    public enum PeerStatus
    {
        NotConnected,
        Connecting,
        Connected,
        Disconnecting,
        Failed
    };
}
