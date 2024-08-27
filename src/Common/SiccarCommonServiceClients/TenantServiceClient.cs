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

using System.Net;
using Siccar.Platform;
using System.Text.Json;
using Siccar.Application;
using Siccar.Common.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Siccar.Common.ServiceClients
{
    public class TenantServiceClient : ITenantServiceClient
    {
        private readonly SiccarBaseClient _baseClient;
        private readonly string _tenantEndpoint = "Tenants";
        private readonly string _clientEndpoint = "clients";


        public TenantServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }

        public async Task<List<Tenant>> All()
        {
            var URI = $"{_tenantEndpoint}";
            var response = await _baseClient.GetJsonAsync(URI);
            var responseData = response.Deserialize<List<Tenant>>(_baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<Tenant> GetTenantById(string id)
        {
            var URI = $"{_tenantEndpoint}/{id}";
            var response = await _baseClient.GetJsonAsync(URI);
            var responseData = response.Deserialize<Tenant>(_baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<Tenant> UpdateTenant(string id, Tenant requestData)
        {
            var URI = $"{_tenantEndpoint}/{id}";
            var response = await _baseClient.PutJsonAsync(URI, JsonSerializer.Serialize(requestData, _baseClient.serializerOptions));
            var contentString = await response.Content.ReadAsStringAsync();

            var responseData = JsonSerializer.Deserialize<Tenant>(contentString, _baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<List<Models.Tenant.Client>> ListClients(string tenantId)
        {
            var response = await _baseClient.GetJsonAsync($"{_tenantEndpoint}/{tenantId}/clients");
            var responseData = response.Deserialize<List<Models.Tenant.Client>>(_baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<Models.Tenant.Client?> Get(string tenantId, string clientId)
        {
            try
            {
                var response = await _baseClient.GetJsonAsync($"{_tenantEndpoint}/{tenantId}/{_clientEndpoint}/{clientId}");
                var responseData = response.Deserialize<Models.Tenant.Client>(_baseClient.serializerOptions);
                return responseData;
            }
            catch (HttpStatusException e) when(e.Status == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Models.Tenant.Client> Create(Models.Tenant.Client client)
        {
            var payload = JsonSerializer.Serialize(client, _baseClient.serializerOptions);
            var responseJson = await _baseClient.PostJsonAsync($"{_tenantEndpoint}/{client.TenantId}/{_clientEndpoint}", payload);
            return JsonSerializer.Deserialize<Models.Tenant.Client>(responseJson, _baseClient.serializerOptions)!;
        }

        public async Task DeleteClient(string tenantId, string clientId)
        {
            await _baseClient.Delete($"{_tenantEndpoint}/{tenantId}/{_clientEndpoint}/{clientId}");
        }

        public async Task<Models.Tenant.Client> ClientUpdate(Models.Tenant.Client client)
        {
            var URI = $"{_tenantEndpoint}/{client.TenantId}/{_clientEndpoint}/{client.ClientId}";
            var jsonData = JsonSerializer.Serialize(client, _baseClient.serializerOptions);
            var response = await _baseClient.PutJsonAsync(URI, jsonData);
            return JsonSerializer.Deserialize<Models.Tenant.Client>(await response.Content.ReadAsStringAsync(), _baseClient.serializerOptions)!;
        }

        public async Task<List<Participant>> GetPublishedParticipants(string registerId)
        {
            var response = await _baseClient.GetJsonAsync($"{_tenantEndpoint}/{registerId}/participants");
            var responseData = response.Deserialize<List<Participant>>(_baseClient.serializerOptions);
            return responseData!;
        }
        public async Task<ODataRaw<List<Participant>>> GetPublishedParticipantsOData(string registerId, string query = "")
        {
            var uri = $"participants({registerId})/participants{query}";
            var response = await _baseClient.GetJsonAsyncOData(uri);
            var responseData = response.Deserialize<ODataRaw<List<Participant>>>(_baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<Participant> GetPublishedParticipantById(string registerId, string participantId)
        {
            var response = await _baseClient.GetJsonAsync($"{_tenantEndpoint}/{registerId}/participant/{participantId}");
            var responseData = response.Deserialize<Participant>(_baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<TransactionModel> PublishParticipant(string registerId, string walletAddress, Participant participant)
        {
            var payload = JsonSerializer.Serialize(participant, _baseClient.serializerOptions);
            var responseJson = await _baseClient.PostJsonAsync($"{_tenantEndpoint}/publishparticipant/{registerId}/{walletAddress}", payload);
            return JsonSerializer.Deserialize<TransactionModel>(responseJson, _baseClient.serializerOptions)!;
        }
    }
}
