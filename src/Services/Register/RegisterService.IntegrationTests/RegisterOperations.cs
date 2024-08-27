using System.Dynamic;
using System.Net;
#nullable enable

namespace RegisterService.IntegrationTests
{
    public class RegisterOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _registerUri = "api/registers";

        public RegisterOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetDockets(string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}/dockets");
        }

        public async Task<HttpResponseMessage> GetDocket(string registerId, ulong docketId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}/dockets/{docketId}");
        }

        public async Task<HttpResponseMessage> GetTransactions(string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}/transactions");
        }

        public async Task<HttpResponseMessage> GetTransaction(string registerId, string transactionId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}/transactions/{transactionId}");
        }

        public async Task<HttpResponseMessage> GetTransactionsByDocket(string registerId, string docketId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}/docket/{docketId}/transactions");
        }

        public async Task<HttpResponseMessage> GetRegisters(bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}");
        }

        public async Task<HttpResponseMessage> GetRegister(string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_registerUri}/{registerId}");
        }

        public async Task<HttpResponseMessage> CreateRegister(dynamic register, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_registerUri}", (object)register);
        }

        public async Task<HttpResponseMessage> DeleteRegister(string registerId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.DeleteAsync($"{_registerUri}/{registerId}");
        }

        private static object GetFakeBearerToken(bool authorised, string? role)
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
