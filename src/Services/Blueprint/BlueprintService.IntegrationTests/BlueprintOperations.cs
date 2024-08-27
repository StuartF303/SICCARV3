using System.Dynamic;
using System.Net;

namespace BlueprintService.IntegrationTests
{
    public class BlueprintOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _blueprintUri = "api/blueprints";

        public BlueprintOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAll(bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_blueprintUri}");
        }

        public async Task<HttpResponseMessage> Get(string id, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_blueprintUri}/{id}");
        }

        public async Task<HttpResponseMessage> Save(dynamic blueprint, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_blueprintUri}", (object)blueprint);
        }

        public async Task<HttpResponseMessage> Publish(string walletAddress, string registerId, dynamic blueprint, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_blueprintUri}/{walletAddress}/{registerId}/publish", (object)blueprint);
        }

        public async Task<HttpResponseMessage> GetAllPublished(string walletAddress, string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_blueprintUri}/{registerId}/published?wallet-address={walletAddress}");
        }

        public async Task<HttpResponseMessage> Update(dynamic blueprint, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PutAsJsonAsync($"{_blueprintUri}/{blueprint.id}", (object)blueprint);
        }

        public async Task<HttpResponseMessage> Delete(string id, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.DeleteAsync($"{_blueprintUri}/{id}");
        }

        private dynamic GetFakeBearerToken(bool authorised, string? role)
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
