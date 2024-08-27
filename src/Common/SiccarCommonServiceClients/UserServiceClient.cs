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

using System.Text.Json;
using Json.More;
using Siccar.Platform;

namespace Siccar.Common.ServiceClients
{
    public class UserServiceClient : IUserServiceClient 
    {
        private readonly SiccarBaseClient _baseClient;
        private const string _usersUri = "users";
        private readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public UserServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }

        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            return await _baseClient.Delete($"{_usersUri}/{id}");
        }

        public async Task<JsonDocument> Get(Guid id)
        {
            return await _baseClient.GetJsonAsync($"{_usersUri}/{id}");
        }

        public async Task<HttpResponseMessage> AddToRole(Guid id, string role)
        {
            var user = await Get(id);
            var existingUser = user.Deserialize<User>(JsonSerializerOptions);
            existingUser?.Roles?.Add(role);
            return await _baseClient.PutJsonAsync($"{_usersUri}/{id}", existingUser.ToJsonDocument());
        }

        public async Task<HttpResponseMessage> RemoveFromRole(Guid id, string role)
        {
            var user = await Get(id);
            var existingUser = user.Deserialize<User>(JsonSerializerOptions);
            existingUser?.Roles?.Remove(role);
            return await _baseClient.PutJsonAsync($"{_usersUri}/{id}", existingUser.ToJsonDocument());
        }

        public async Task<List<User>> All()
        {
            var URI = $"{_usersUri}";
            var response = await _baseClient.GetJsonAsync(URI);
            var responseData = response.Deserialize<List<User>>(_baseClient.serializerOptions);
            return responseData!;
        }
    }
}
