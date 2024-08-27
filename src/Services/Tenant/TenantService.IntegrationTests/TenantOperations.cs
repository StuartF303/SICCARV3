using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using Siccar.Platform;

namespace TenantService.IntegrationTests
{
    public class TenantOperations
    {
        private readonly HttpClient _httpClient;
        private readonly string _tenantUri = "api/tenants";

        public TenantOperations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> ListTenants(bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_tenantUri}");
        }

        public async Task<HttpResponseMessage> GetAllClients(string tenantId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_tenantUri}/{tenantId}/clients");
        }

        public async Task<HttpResponseMessage> GetClient(string tenantId, string clientId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_tenantUri}/{tenantId}/clients/{clientId}");
        }

        public async Task<HttpResponseMessage> UpdateClient(string tenantId, dynamic client, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PutAsJsonAsync($"{_tenantUri}/{tenantId}/clients/{client.clientId}", (object)client);
        }

        public async Task<HttpResponseMessage> GetTenant(string tenantId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.GetAsync($"{_tenantUri}/{tenantId}");
        }

        public async Task<HttpResponseMessage> CreateTenant(dynamic tenant, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_tenantUri}", (object)tenant);
        }

        public async Task<HttpResponseMessage> UpdateTenant(dynamic tenant, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PutAsJsonAsync($"{_tenantUri}/{tenant.id}", (object)tenant);
        }

        public async Task<HttpResponseMessage> DeleteTenant(string tenantId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.DeleteAsync($"{_tenantUri}/{tenantId}");
        }

        public async Task<HttpResponseMessage> CreateClient(string tenantId, dynamic client, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_tenantUri}/{tenantId}/clients", (object)client);
        }

        public async Task<HttpResponseMessage> DeleteClient(string tenantId, string clientId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.DeleteAsync($"{_tenantUri}/{tenantId}/Clients/{clientId}");
        }

        public async Task<HttpResponseMessage> PublishParticipant(string registerId, string walletAddress, dynamic participant, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_tenantUri}/PublishParticipant/{registerId}/{walletAddress}", (object)participant);
        }

        public async Task<HttpResponseMessage> GetPublishedParticipantById(string registerId, string participantId, bool authorised = true, string? role = null)
        {
            _httpClient.SetFakeBearerToken((object)GetFakeBearerToken(authorised, role));
            return await _httpClient.PostAsJsonAsync($"{_tenantUri}/{registerId}/participant/{participantId}", (object)participantId);
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
