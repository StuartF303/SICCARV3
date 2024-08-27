using Makaretu.Dns;
using Microsoft.Extensions.Logging;
using Siccar.Network.Peers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Router
{
    public class MulticastDiscovery
    {
        private ILogger Log = null;
        private const string ServiceName = "_ddr"; // Service Type is a Distributed Digital Register
        private const string serviceType = "_tcp.local";
        private MulticastService _mdns = null;
        private ServiceDiscovery _serviceDiscovery = null;
        private ServiceProfile _profile = null;
        private Peer _thisPeer = null;
        private readonly IPeerRouter _router = null;

        public MulticastDiscovery(IPeerRouter Router, ILogger logger)
        {
            Log = logger;
            _router = Router;

        }

        public void StartAdvertising(Peer thisPeer)
        {
            _thisPeer = thisPeer;

            _profile = new ServiceProfile(_thisPeer.PeerName.ToLower(),
                                               ServiceName + "._tcp",
                                               (ushort)_thisPeer.IPSocket,
                                               new IPAddress[] { _thisPeer.IPEndPoint });

            try
            {
                _mdns = new MulticastService();
                _serviceDiscovery = new ServiceDiscovery(_mdns);

                _serviceDiscovery.ServiceInstanceDiscovered += (sender, discargs) => { peerServiceInstanceDiscovered(sender, discargs); };
                _serviceDiscovery.ServiceInstanceShutdown += (sender, shutargs) => { peerServiceInstanceShutdown(sender, shutargs); };


                _serviceDiscovery.Advertise(_profile);

                Log.LogInformation("[P2P:mDNS] Advertising peer to local network as : {0}", _profile.InstanceName);
            }
            catch (Exception er)
            {
                Log.LogInformation("[P2P:mDNS] Failed to start mDNS : {0}", er.Message);
            }

        }

        public void peerServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs e)
        {
            //right we are going to get all sorts of inbound here
            //only interested in host[0] and '_ddr'[1]
            if (e.ServiceInstanceName.Labels[1] == _thisPeer.PeerName)
            {
                string remotePeerName = e.ServiceInstanceName.Labels[0] ?? "";
                if ((remotePeerName.ToLower() != _thisPeer.PeerName.ToLower()))
                {

                    var rr = e.Message.AdditionalRecords;
                    if (rr.Count > 2) // we should get 3 records back
                    {
                        // protocode until I understand the rr's
                        ARecord addrec = (ARecord)rr.Where(t => t.Type == DnsType.A).First(); // if IP v6 then an AAAA record !!
                        SRVRecord srvrec = (SRVRecord)rr.Where(t => t.Type == DnsType.SRV).First();
                        TXTRecord txtrec = (TXTRecord)rr.Where(t => t.Type == DnsType.TXT).First();

                        Peer newPeer = new Peer()
                        {
                            PeerName = srvrec.Name.ToString(),
                            IPSocket = srvrec.Port
                        };

                        if (txtrec.Strings.Where(t => t.Contains("DisableTLS")).Count() > 0)
                        {
                            newPeer.DisabledSSL = true;
                        }

                        // is this host already connected? otherwise basic AddPeer

                        if (_router.AddPeer(newPeer).Result)
                            // should fire this off to an Add Peer Once I get the details
                            Log.LogInformation("[P2P:mDNS] Local discovery of peer : {0}", e.ServiceInstanceName.Labels[0]);
                    }
                }
            }
        }


        /// <summary>
        /// mdns Service Service Shutdown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void peerServiceInstanceShutdown(object sender, ServiceInstanceShutdownEventArgs e)
        {
            //_serviceDiscovery.ServiceInstanceDiscovered;
           
            Log.LogInformation("[P2P:mDNS] Notified of Shutdown for peer : {0}", e.ServiceInstanceName.Labels[0]);
        }

    }
}
