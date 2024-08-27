using System;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Utilities
{
    public partial class Utilities
    {
        public async Task<IPAddress> ExternalIP()
        {
            HttpClient client = new HttpClient();

            var uri = "http://api.ipify.org/"; // probably ought to take this as a configuration variable incase folks dont want to use this service
            var response = await client.GetStringAsync(uri);
            IPAddress addr = IPAddress.Parse(response);

            Log.LogDebug("[P2P] This Peer looked up its Public IP : {0}", response);

            return addr;
        }
    }
}