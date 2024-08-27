using System;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Siccar.Network.Peers.Utilities
{
    public partial class Utilities
    {
        /// <summary>
        /// Returns the Local IP address - more complicated than it sounds
        /// </summary>
        /// <returns></returns>
        public async Task<IPAddress> LocalIP()
        {
            IPAddress ipaddr = new(new byte[] { 0, 0, 0, 0 });
            IPAddress defgw = GetDefaultGateway() ?? new IPAddress(new byte[] { 0, 0, 0, 0 });
        //    IPAddress defnet =  IPNetwork.Parse(defgw.ToString()).Network;
            IPHostEntry hosts = Dns.GetHostEntry(Dns.GetHostName());

            // we should be looking for only 1 IP address and it should be on the same subnet as the default gateway
            // going to start with an assumption that we are on a class 'C'
            foreach (IPAddress ip in hosts.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                  //  IPAddress subnet = IPNetwork.Parse(ip.ToString()).Network;
                  //  if (subnet.Equals(defnet))
                   //     ipaddr = ip;
                }
            }
            await Task.Run(() => Log.LogDebug("[P2P] Private Address : {ip}", ipaddr.ToString()));
            await Task.Run(() => Log.LogDebug("[P2P] Default Gateway : {gw}", defgw.ToString()));
            return ipaddr;
        }

        public static IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                // .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                // .Where(a => Array.FindIndex(a.GetAddressBytes(), b => b != 0) >= 0)
                .FirstOrDefault();
        }
    }
}