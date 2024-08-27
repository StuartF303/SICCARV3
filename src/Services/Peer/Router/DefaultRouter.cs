using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siccar.Platform;
using Siccar.Network.Peers.Core;
using System.Net;
using Siccar.Common;
using System.Text.Json;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Siccar.Network.Peers.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace Siccar.Network.Router
{

    public class DefaultRouter : IPeerRouter
    {
        private readonly Peer _thisHost = null;
        private readonly List<Peer> _peers = new();
        private bool _online = false;

        private readonly ILogger<DefaultRouter> Log = null;
        private readonly IHttpClientFactory HttpClientFactory = null;
        private readonly IConfiguration Configuration = null;
        private readonly IFactory<ITransportConnectionHandler> _handler = null;
        private MulticastDiscovery _mdns = null;

        readonly JsonSerializerOptions jsOpts = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// A System provided Simple Peer Router
        /// It maintains a list of connected Peers and Transport to those Peers
        /// When there is traffic to & from the network this service is responsible 
        /// to getting and sending to the intneral service busses via DAPR
        /// </summary>
        /// <param name="HttpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultRouter(IConfiguration configuration, IHttpClientFactory httpClientFactory,
            IFactory<ITransportConnectionHandler> handlerFactory, ILogger<DefaultRouter> logger)
        {
            this.Log = logger;
            this.HttpClientFactory = httpClientFactory;
            this.Configuration = configuration;
            this._handler = handlerFactory;


            try
            {
                var hostname = IPGlobalProperties.GetIPGlobalProperties().HostName.ToLower(); ;
                var domainname = IPGlobalProperties.GetIPGlobalProperties().DomainName.ToLower();
                IPAddress hostAddress = IPAddress.Loopback;


                bool useinternalip = bool.Parse(string.IsNullOrWhiteSpace(Configuration["Peer:UseInternalIP"]) ? "false" : Configuration["Peer:UseInternalIP"]);

                if (!string.IsNullOrWhiteSpace(Configuration["Peer:IPEndPoint"]))
                    hostAddress = IPAddress.Parse(Configuration["Peer:IPEndPoint"]);
                else
                if (!useinternalip)
                { // get external IP
                    var utils = new Utilities(Log, Configuration);
                    hostAddress = utils.ExternalIP().Result;
                }
                else
                { // get the internal IP

                    var utils = new Utilities(Log, Configuration);
                    hostAddress = utils.LocalIP().Result;
                }

                Log.LogInformation("[P2P] This Peer using IP : {host}", hostAddress.ToString());

                int hostPort = int.Parse(string.IsNullOrEmpty(Configuration["Peer:OverrideIPPort"]) ? "443" : Configuration["Peer:OverrideIPPort"]);

                bool isHttps = !bool.Parse(string.IsNullOrEmpty(Configuration["Peer:DisableTLS"]) ? "false" : Configuration["Peer:DisableTLS"]);

                try
                {
                    var name = string.IsNullOrEmpty(Configuration["Peer:Name"]) ? hostname + "." + domainname : Configuration["Peer:Name"];
                    var uri = (isHttps ? "https://" : "http://") + name;
                    if (hostPort != 443)
                        uri += ":" + hostPort.ToString();

                    this._thisHost = new Peer()
                    {
                        PeerName = name,
                        URIEndPoint = new Uri(uri),
                        IPEndPoint = hostAddress,
                        IPSocket = hostPort,
                        DisabledSSL = !isHttps,
                        Status = PeerStatus.Connected
                    };
                    _online = true;
                }
                catch (SocketException e)
                {
                    Log.LogError("[ROUTER] Cant configure this peer : {msg}", e.Message);
                }
            }
            catch (Exception er)
            {
                Log.LogError(er, "Error is Peer Configuration : {msg} at {st}", er.Message, er.StackTrace);
            }

        }

        /// <summary>
        /// Network Register Table a long running record of  what Peers have been seen on the Network with the Registers they serve
        /// </summary>
        public IDictionary<Peer, Register> NetworkRegisterTable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// The current connected Peers 
        /// </summary>
        public IEnumerable<Peer> ActivePeers { get { return _peers; } set => throw new NotSupportedException("Use Router Methods"); }

        /// <summary>
        /// A readonly view of self.
        /// </summary>
        public Peer Self { get { return _thisHost; } set => throw new NotSupportedException("Sorry, cant do that"); }

        /// <summary>
        /// Sets up the local Peer from config and discoverd information about the host.
        /// </summary>
        /// <returns></returns>
        public async Task InitialisePeer()
        {
            // ok startup tasks 
            // 0) If name is local then it hasnt been set so try to discover my name 

            // 1) Set Self Online




            // 2) Advertise as approptiate
            try
            {
                if (bool.Parse(Configuration["Peer:mDNSEnabled"] ?? "true"))
                {
                    // start the mDNS broadcasts
                    this._mdns = new MulticastDiscovery(this, Log);
                    this._mdns.StartAdvertising(Self);
                }
            }
            catch
            {
                Log.LogError("Peer Configuration : AdvertiseLocally cannot be parsed.");
            }

            // 3) Discover from seed  
            await DiscoverPeers();


        }

        /// <summary>
        /// Disconnect all peers and write state
        /// 
        /// </summary>
        public void Shutdown()
        {
            if (_peers.Count > 0)
            {
                var ropeers = _peers; // stops collection modification er
                foreach (var p in ropeers)
                {
                    p.Connection.CloseConnection();
                }
            }
        }

        /// <summary>
        /// Probe and scan for new Peers
        /// </summary>
        /// <returns></returns>
        public async Task DiscoverPeers()
        {
            // 1) Probe Existing known peerss
            // 2) If no known peers use seed peer - can be overridden by config

            if (!string.IsNullOrEmpty(Configuration["Peer:Seed"]))
            {
                await Task.Run(() => AddPeer(new Peer()
                {
                    PeerName = Configuration["Peer:Seed"],
                    URIEndPoint = new Uri(Configuration["Peer:Seed"])
                }));
            }
        }

        /// <summary>
        /// Add a new Peer to the Active Connections
        /// </summary>
        /// <param name="RemotePeer"></param>
        /// <returns></returns>
        public async Task<bool> AddPeer(Peer RemotePeer, bool refresh = false)
        {
            // TODO : Checking etc
            // do some basic checks - is whitelisted / blacklisted
            // can we get some status data from it, is it NATd
            // if we fail return false

            if ((!_online) | (_thisHost == null))
            {
                Log.LogError("Service NOT ONLINE : Probe from {0}", RemotePeer.Id);
                return false;
            }

            if (RemotePeer.PeerName.Contains(_thisHost.PeerName))
                return true; // just dont add it to the list

            // they might already be connected 
            if (ActivePeers.Contains<Peer>(RemotePeer, new PeerComparer()))
            {
                if (refresh) // lets just update what we know about the Peer
                {
                    var updatePeer = await ProbePeer(RemotePeer);

                    // replace the local copy of the Peer
                    var idx = _peers.FindIndex(p => p.PeerName == RemotePeer.PeerName); // should use connectionID
                    if (idx >= 0)
                    {
                        _peers[idx].LastSend = DateTime.UtcNow;
                        _peers[idx].Registers = updatePeer.Registers;
                    }
                }

                return true; // 
            }

            if (_thisHost.Blacklist.Contains<Peer>(RemotePeer, new PeerComparer()))
                return false; // its blacklisted

            // we should check if its whitelisted but it still has to be reachable
            try
            {
                Peer candidatePeer = await ProbePeer(RemotePeer);

                if (candidatePeer == null)
                {
                    candidatePeer = RemotePeer;
                    candidatePeer.ReadOnly = true;
                }


                Log.LogInformation("[P2P] Adding Peer : {peer} : {ep} : RO {ro}", candidatePeer.PeerName, candidatePeer.URIEndPoint, candidatePeer.ReadOnly);
                // otherwise we can communicate with the host and add it to our active list
                candidatePeer.LastConnectedTime = DateTime.UtcNow;
                if (await ConnectPeer(candidatePeer))
                    return true;
            }
            catch (SocketException sktErr)
            {
                Log.LogInformation("[P2P] Could NOT Connect Peer : {peer} : {ep} : {msg} ", RemotePeer.PeerName, RemotePeer.URIEndPoint, sktErr.Message);
            }
            catch (Exception er)
            {
                Log.LogError("[P2P] Exception while Connect Peer : {peer} : {ep} : {msg} ", RemotePeer.PeerName, RemotePeer.URIEndPoint, er.Message);
            }
            return false;
        }


        /// <summary>
        /// Remove a Peer from the Active connections
        /// </summary>
        /// <param name="RemotePeer"></param>
        /// <returns></returns>
        public void RemovePeer(Peer RemotePeer)
        {
            _peers.RemoveAll(p => p.Id == RemotePeer.Id);
        }


        #region Probes
        /// <summary>
        /// Pass a peer to get its details - null if not accessible
        /// will boil this down later - should do updates if passed an ID etc
        /// </summary>
        /// <param name="RemotePeer"></param>
        /// <returns></returns>
        public async Task<Peer> ProbePeer(Peer RemotePeer)
        {
            if (Uri.CheckHostName(RemotePeer.URIEndPoint.Host) == UriHostNameType.Dns)
            {
                // apparently we can resolve this so lets try
                return await ProbePeer(RemotePeer.URIEndPoint);
            }

            if (RemotePeer.IPEndPoint != null)
            {
                if (IPEndPoint.TryParse(RemotePeer.IPEndPoint.ToString(), out _))
                {
                    // we have a valid IP Endpoint
                    Uri rn = new(String.Format("https://{0}:{1}", RemotePeer.IPEndPoint, RemotePeer.IPSocket));
                    return await ProbePeer(rn);
                }
            }
            return null;
        }

        public async Task<Peer> ProbePeer(Uri RemoteUri)
        {
            using var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("PeerId", this.Self.Id); // must remember to add this to all peer requests or they will be knocked back
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.BaseAddress = RemoteUri;

            try
            {
                var response = await client.GetAsync(Constants.PeerAPIURL);

                string body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Peer>(body, jsOpts);

                Log.LogDebug("[P2P] Probing {uri} : {result}", RemoteUri, result);
                return result;
            }
            catch (HttpRequestException httpErr)
            {
                Log.LogWarning("[P2P] Could Not Probe {uri}, {msg}", RemoteUri, httpErr.Message);
            }
            catch (TaskCanceledException taskErr)
            {
                Log.LogInformation("[P2P] Failed Probe {uri}, {msg}", RemoteUri, taskErr.Message);
            }

            return null;
        }


        #endregion

        /// <summary>
        /// Ask a remote peer to connect to us
        /// </summary>
        /// <param name="RemotePeer"></param>
        /// <returns></returns>
        public async Task RequestConnection(Peer RemotePeer)
        {
            using var client = HttpClientFactory.CreateClient();
            client.BaseAddress = RemotePeer.URIEndPoint;
            client.DefaultRequestHeaders.Add("PeerId", this.Self.Id); // must remember to add this to all peer  requests or they will be knocked back
            var request = new HttpRequestMessage(HttpMethod.Post, Constants.PeerAPIURL);

            request.Headers.Add("Accept", "application/json");

            request.Content = new StringContent(JsonSerializer.Serialize(Self));
            request.Content.Headers.Clear(); // no this is JSON

            request.Content.Headers.Add("Content-Type", "application/json");

            try
            {
                var response = await client.SendAsync(request);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        try
                        {
                            string body = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(body))
                            {
                                var result = JsonSerializer.Deserialize<Peer>(body, jsOpts);
                            }
                            Log.LogDebug("[P2P] Reciprocal Connection {peer} ", RemotePeer);

                        }
                        catch (HttpRequestException httpErr)
                        {
                            Log.LogWarning("[P2P] Reciprocal Failed {peer}, {msg}", RemotePeer, httpErr.Message);
                        }
                        break;

                    case HttpStatusCode.InternalServerError:
                        Log.LogError("[P2P] Remote Peer returned Internal Server Error");
                        break;

                    default:
                        Log.LogWarning("[P2P] Unknown reponse from Reciprocal Peer : {name}", RemotePeer.PeerName);
                        break;
                }
            }
            catch (HttpRequestException httpErr)
            {
                Log.LogInformation("[P2P] Failed to connect to {address} reason: {msg}", client.BaseAddress, httpErr.Message);
            }
        }

        /// <summary>
        /// Creates the Connection to a tested peer
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        public async Task<bool> ConnectPeer(Peer peer)
        {
            // what if the peer is readonly?

            peer.Connection = _handler.Create();

            await peer.Connection.OpenConnection(peer); // could be nicer
            //peer.Registers = await GetPeerRegisters(peer);
            _peers.Add(peer);


            // So we are connected one way, we should try the reciprocal connection
            //if (!_peers.Contains(peer, new PeerComparer()))
            await RequestConnection(peer);

            return true;
        }

        /// <summary>
        /// Probe and update a Peer
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        public async Task UpdatePeer(Peer peer)
        {
            var updatedPeer = await ProbePeer(peer);

            var lcl = _peers.FindIndex(p => p.Id == peer.Id);

            _peers[lcl].Registers = updatedPeer.Registers;

        }

        /// <summary>
        /// Tell our Peers to check us out, we have changed...
        /// Probaby something major
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePeers()
        {
            foreach (var p in _peers)
            {
                // tell them to update thier view of use
                await p.Connection.UpdateConnection();
                // we update our view of them

            }
        }

        /// <summary>
        /// When a Remote connection is lost we need to report it to the router
        /// </summary>
        /// <param name="ConnectionId"></param>
        public void DisconnectPeer(string ConnectionId)
        {
            if (_peers.Count > 0)
            {
                var peer = _peers.FirstOrDefault(p => p.Connection.Id == ConnectionId);
                if (peer != null)
                {
                    // send disconnect notice to be nice
                    // Delete {peer}/api/peer/{id}

                    RemovePeer(peer);
                    Log.LogInformation("[P2P] Remote Peer {id} : {name} disconnected", peer.Id, peer.PeerName);
                }
                else
                {
                    // this is a warning as it should never happen
                    Log.LogWarning("[P2P] Dangling Remote Peer ConnectionID {id} disconnected", ConnectionId);
                }
            }
            else
                Log.LogInformation("[P2P] Dangling Remote Peer ConnectionID {id} disconnected", ConnectionId);
        }



        /// <summary>
        /// Send the Register message to the Peers who care
        /// </summary>
        /// <param name="RegisterId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task BroadcastMessage(string RegisterId, PeerMessage message)
        {
            await Task.Run(() => Parallel.ForEach(_peers.Where(p => p.Registers.Contains(RegisterId)),
                async pr =>
                {
                    try
                    {
                        await pr.Connection.SendMessage(message);
                    }
                    catch (Exception er)
                    {
                        Log.LogWarning("[P2P] Error in sending broadcast message for {id} : {msg}", RegisterId, er.Message);
                    }
                }));
        }

        private async Task<List<Register>> GetPeerRegisters(Peer RemotePeer)
        {
            List<Register> remoteRegisters = new();


            using (var client = HttpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Add("PeerId", this.Self.Id); // must remember to add this to all peer requests or they will be knocked back
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.BaseAddress = RemotePeer.URIEndPoint;

                try
                {
                    var response = await client.GetAsync(Constants.RegisterAPIURL);

                    string body = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(body))
                    {
                        remoteRegisters = JsonSerializer.Deserialize<ODataResponse<Register>>(body, jsOpts).Value;

                    }

                    Log.LogDebug("[P2P] Retrieving Register List from {peer} ", RemotePeer);
                }
                catch (HttpRequestException httpErr)
                {
                    Log.LogWarning("[P2P] Could Not Probe {peer}, {msg}", RemotePeer, httpErr.Message);
                }
            }

            return remoteRegisters;
        }

        internal class ODataResponse<T>
        {
            public List<T> Value { get; set; }
        }
    }
}
