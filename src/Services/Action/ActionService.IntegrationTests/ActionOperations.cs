using System.Dynamic;
using System.Net;
using Siccar.Common;

namespace ActionService.IntegrationTests
{
    public class ActionOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _actionUri = "api/Actions";

        public ActionOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetStartingActions(string registerId, string walletAddress, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_actionUri}/{walletAddress}/{registerId}/blueprints");
        }

        public async Task<HttpResponseMessage> GetAll(string registerId, string walletAddress, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_actionUri}/{walletAddress}/{registerId}");
        }

        public async Task<HttpResponseMessage> GetById(string registerId, string walletAddress, string transactionId, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_actionUri}/{walletAddress}/{registerId}/{transactionId}");
        }

        public async Task<HttpResponseMessage> Submit(dynamic? actionSubmission, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync(_actionUri, (object)actionSubmission!);
        }

        private static object GetFakeBearerToken(bool authorised, string role)
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
