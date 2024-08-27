/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

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
