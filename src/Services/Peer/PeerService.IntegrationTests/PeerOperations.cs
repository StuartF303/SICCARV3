using System.Dynamic;
using System.Net;
using Siccar.Common;

namespace PeerService.IntegrationTests
{
    public class PeerOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _peerUri = "api/peer";

        public PeerOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetPeer(bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_peerUri}");
        }

        public async Task<HttpResponseMessage> GetPeers(bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_peerUri}/peers");
        }

        public async Task<HttpResponseMessage> PostPeer(dynamic peer, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_peerUri}", (object)peer);
        }

        public async Task<HttpResponseMessage> PostHostRegister(string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsync($"{_peerUri}/HostRegister/{registerId}", null);
        }
        
        private object GetFakeBearerToken(bool authorised, string? role)
        {
            var id = "9e3163b9-1ae6-4652-9dc6-7898ab7b7a00";
            dynamic data = new ExpandoObject();
            data.sub = id;
            if (authorised)
            {
                data.role = new[] { role };
            }
            data.tenant = "test-tenant";
            data.name = id;
            data.unique_name = id;

            return data;
        }
    }
}
