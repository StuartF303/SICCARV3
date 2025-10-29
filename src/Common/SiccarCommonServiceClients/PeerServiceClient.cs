// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siccar.Network.Peers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Siccar.Common.ServiceClients
{
    public class PeerServiceClient
    {
        private SiccarBaseClient _baseClient;
        private string _endpoint = "peer";

        public PeerServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }

        
        public async Task<JsonDocument> GetPeerInfo()
        {
            var peerInfo = await _baseClient.GetJsonAsync(_endpoint);

            return peerInfo; ;
        }

        public async Task<JsonDocument> GetPeersInfo()
        {
            var peersInfo = await _baseClient.GetJsonAsync(_endpoint + "/peers");

            return peersInfo; ;
        }

        public async Task<string> RequestAddPeer( JsonDocument newPeer )
        {
            var peerconnect = await _baseClient.PostJsonAsync(_endpoint, newPeer);
            return peerconnect;
        }

        public async Task<bool> HostRegister(string registerId)
        {
            var hostInfo = await _baseClient.PostJsonAsync(_endpoint + "/hostregister/" + registerId, "");

            if (hostInfo == "ok")
                return true;
            else
                return false;

        }
    }
}
