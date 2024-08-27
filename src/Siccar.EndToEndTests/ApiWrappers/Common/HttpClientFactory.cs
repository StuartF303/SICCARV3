using System.Text.Json.Nodes;

namespace Siccar.EndToEndTests.Common
{
    internal class HttpClientFactory
    {
        private static HttpClient? _tenantHttpClient;
        private static HttpClient? _serviceHttpClient;

        public static async Task<HttpClient> Create()
        {
            if (_tenantHttpClient == null)
            {
                _tenantHttpClient ??= new HttpClient();
                _tenantHttpClient.BaseAddress = new Uri("https://localhost:8443");
            }

            if (_serviceHttpClient == null)
            {
                var formContent = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "client_credentials"),
                    new("scope", "wallet.admin"),
                    new("client_id", "siccar-integration-client"),
                    new("client_secret", "secret")
                };

                var req = new HttpRequestMessage(HttpMethod.Post, "connect/token") { Content = new FormUrlEncodedContent(formContent) };
                var bearerResult = await _tenantHttpClient.SendAsync(req);

                var bearerData = await bearerResult.Content.ReadAsStringAsync();
                var bearerToken = JsonNode.Parse(bearerData)!.AsObject()["access_token"]!.GetValue<string>();

                _serviceHttpClient ??= new HttpClient();
                _serviceHttpClient.BaseAddress = new Uri("https://localhost:8443");
                _serviceHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
            }

            return _serviceHttpClient;
        }
    }
}
