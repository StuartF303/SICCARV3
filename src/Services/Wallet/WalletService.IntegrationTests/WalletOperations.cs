using System.Dynamic;
using System.Net;
using System.Text;
using Json.More;
using Siccar.Common;
using WalletService.IntegrationTests.Models;

namespace WalletService.IntegrationTests
{
    public class WalletOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _walletUri = "api/wallets";

        public WalletOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> Create(CreateWallet createWallet, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync(_walletUri, createWallet);
        }

        public async Task<HttpResponseMessage> Update(string address, UpdateWallet updateWallet, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PatchAsync($"{_walletUri}/{address}", new StringContent(updateWallet.ToJsonDocument().ToString()!, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Get(string address, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_walletUri}/{address}");
        }

        public async Task<HttpResponseMessage> ListAll(string role, bool allInTenant, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_walletUri}?allinTenant={allInTenant}");
        }

        public async Task<HttpResponseMessage> Delete(string address, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.DeleteAsync($"{_walletUri}/{address}");
        }

        public async Task<HttpResponseMessage> GetTransactions(string address, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_walletUri}/{address}/transactions");
        }

        public async Task<HttpResponseMessage> GetTransaction(string address, string id, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_walletUri}/{address}/transactions/{id}");
        }

        public async Task<HttpResponseMessage> CreateTransaction(string address, CreateTransaction createTransaction, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync($"{_walletUri}/{address}/transactions", createTransaction);
        }

        public async Task<HttpResponseMessage> DecryptTransactionPayloads(string address, dynamic transaction, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync($"{_walletUri}/{address}/transactions/decrypt", (object)transaction);
        }

        public async Task<HttpResponseMessage> Validate(string address, dynamic transaction, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync($"{_walletUri}/{address}/transactions/validate", (object)transaction);
        }

        public async Task<HttpResponseMessage> GetDelegates(string address, dynamic transaction, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.GetAsync($"{_walletUri}/{address}/delegates");
        }

        public async Task<HttpResponseMessage> AddDelegate(string address, WalletAccess walletAccess, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PostAsJsonAsync($"{_walletUri}/{address}/delegates", walletAccess);
        }

        public async Task<HttpResponseMessage> RemoveDelegate(string address, string subject, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.DeleteAsync($"{_walletUri}/{address}/{subject}/delegates");
        }

        public async Task<HttpResponseMessage> UpdateDelegate(string address, WalletAccess walletAccess, bool authorised = true)
        {
            _httpClient.SetFakeBearerToken(GetFakeBearerToken(authorised, Constants.WalletUserRole));
            return await _httpClient.PatchAsync($"{_walletUri}/{address}/delegates", new StringContent(walletAccess.ToJsonDocument().ToString()!, Encoding.UTF8, "application/json"));
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
            data.tenant = "test-tenants";
            data.name = id;
            data.unique_name = id;

            return data;
        }
    }
}
