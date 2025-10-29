// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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