// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

#nullable enable

namespace Siccar.Common.Adaptors
{
	public class HttpClientAdaptor : IHttpClientAdaptor
	{
		private readonly HttpClient _httpClient;

		public string? BaseURI
		{
			get
			{
				return _httpClient.BaseAddress?.AbsoluteUri;
			}
		}

		public HttpClientAdaptor(HttpClient httpClient) { _httpClient = httpClient; }

		public async Task<bool> TryAddAuthHeader(IHttpContextAccessor httpContextAccessor)
		{
			var token = StringValues.Empty;
			var hasAuthHeader = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out token) ?? false;

			if (!hasAuthHeader && httpContextAccessor?.HttpContext != null)
			{
				token = $"bearer {await httpContextAccessor?.HttpContext?.GetTokenAsync("access_token")!}";
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				return false;
			}

			var jwtToken = token.ToString().Remove(0, 7);
			_httpClient.DefaultRequestHeaders.Authorization = new("bearer", jwtToken);
			return true;
		}

		public void AddAuthHeader(string accessToken)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
		}

		public async Task<HttpResponseMessage> GetAsync(string requestUri)
		{
			return await _httpClient.GetAsync(requestUri);
		}

		public async Task<HttpResponseMessage> PostAsync(string requestUri, string payload)
		{
			var resp = await _httpClient.PostAsync(requestUri, new StringContent(payload, Encoding.UTF8, "application/json"));
			return resp; 
		}

        public async Task<HttpResponseMessage> PostAsync(string requestUri, MultipartFormDataContent payload)
        {
            var resp = await _httpClient.PostAsync(requestUri, payload);
            return resp;
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, string payload)
        {
            var resp = await _httpClient.PatchAsync(requestUri, new StringContent(payload, Encoding.UTF8, "application/json"));
            return resp;
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, string payload)
		{
			var response = await _httpClient.PutAsync(requestUri, new StringContent(payload, Encoding.UTF8, "application/json"));
			return response;
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			return await _httpClient.SendAsync(request);
		}

		public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
		{
			return await _httpClient.DeleteAsync(requestUri);
		}
	}
}
