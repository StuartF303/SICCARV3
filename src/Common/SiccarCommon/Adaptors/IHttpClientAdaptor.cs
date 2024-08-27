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
using System.Net.Http;
using System.Threading.Tasks;

namespace Siccar.Common.Adaptors
{
    public interface IHttpClientAdaptor
    {
        public string BaseURI { get; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        public Task<HttpResponseMessage> GetAsync(string requestUri);

        public Task<HttpResponseMessage> PostAsync(string requestUri, string payload);
        public Task<HttpResponseMessage> PostAsync(string requestUri, MultipartFormDataContent payload);

        public Task<HttpResponseMessage> PatchAsync(string requestUri, string payload);
        
        public Task<HttpResponseMessage> PutAsync(string requestUri, string payload);

        public Task<HttpResponseMessage> DeleteAsync(string requestUri);

        public Task<bool> TryAddAuthHeader(IHttpContextAccessor httpContextAccessor);

        public void AddAuthHeader(string accessToken);
    }
}