using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siccar.Common.Adaptors;
using Siccar.Common.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Siccar.Common.ServiceClients
{
    public class ODataRaw<T> where T : new()
    {
        [JsonPropertyName("@odata.context")]
        public string MetaPath { get; set; } = string.Empty;
        [JsonPropertyName("@odata.count")]
        public int MetaCount { get; set; } = 0;
        public T Value { get; set; } = new T();
    }
    public class SiccarBaseClient : ISiccarServiceClient
    {
        public readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
        public string ServiceHost = "https://localhost:8443/";
        protected string _baseServiceUrl = "api/";
        protected string _baseServiceUrlOData = "odata/";
        protected readonly IHttpClientAdaptor _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IAccessTokenProvider? _accessTokenProvider;
        private readonly UserAuthentication? _userAuthentication;
        public readonly ILogger<SiccarBaseClient> Log;
        private static string _bearer = "";
        private static bool _useSpecifiedBearer;

        // Method to instanciate for user/app clients
        public SiccarBaseClient(
            IHttpClientFactory clientFactory,
            ILogger<SiccarBaseClient> logger,
            IHttpContextAccessor context,
            IAccessTokenProvider accessTokenProvider,
            UserAuthentication userAuthentication)
        {
            _httpClient = new HttpClientAdaptor(clientFactory.CreateClient("siccarClient"));
            _userAuthentication = userAuthentication;
            ServiceHost = _httpClient.BaseURI ?? "https://localhost:8443/";
            _baseServiceUrl = ServiceHost + "api/";
            _baseServiceUrlOData = ServiceHost + "odata/";
            _httpContextAccessor = context;
            Log = logger;
            _accessTokenProvider = accessTokenProvider;
            serializerOptions.Converters.Add(new JsonStringEnumConverter());
            Log.LogDebug("SiccarBaseClient configured from DI");
        }

        // Method to instanciate for service/dapr clients
        // Get additional servcie from DI
        public SiccarBaseClient(IHttpClientAdaptor httpClient, IServiceProvider services)
        {
            _httpClient = httpClient;
            ServiceHost = httpClient.BaseURI ?? ServiceHost;
            _baseServiceUrl = string.IsNullOrWhiteSpace(ServiceHost) ? "/" : ServiceHost;
            _baseServiceUrl += "api/";
            _httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            _userAuthentication = services.GetService<UserAuthentication>();
            _accessTokenProvider = services.GetService<IAccessTokenProvider>();
            serializerOptions.Converters.Add(new JsonStringEnumConverter());
            Log = (ILogger<SiccarBaseClient>?)services.GetService(typeof(ILogger<SiccarBaseClient>))!;
        }

        /// <summary>
        /// Called to ensure the correct Bearer is applied as Authenticatib
        /// </summary>
        public async Task SetBearerAsync(string bearer = "")
        {
            if (string.IsNullOrWhiteSpace(_bearer))
            {
                _bearer = bearer;
                // So we don't use the cached access token, because there may be one on disk
                _useSpecifiedBearer = !string.IsNullOrEmpty(bearer);
            }

            if (_accessTokenProvider != null)
            {
                var accessTokenResult = await _accessTokenProvider.RequestAccessToken();
                if (accessTokenResult != null)
                {
                    accessTokenResult.TryGetToken(out var token);
                    _httpClient.AddAuthHeader(token?.Value);
                }
                else if (await _httpClient.TryAddAuthHeader(_httpContextAccessor))
                {
                    //token already added in condition.
                }
                else
                {
                    if (!_useSpecifiedBearer && !string.IsNullOrWhiteSpace(_userAuthentication?.AuthCache?.AccessToken))
                    {
                        _httpClient.AddAuthHeader(_userAuthentication?.AuthCache?.AccessToken);
                    }
                    else if (_useSpecifiedBearer && !string.IsNullOrWhiteSpace(_bearer))
                    {
                        _httpClient.AddAuthHeader(_bearer);
                    }
                }
            }
            else
            {
                if (!await _httpClient.TryAddAuthHeader(_httpContextAccessor))
                    Log.LogDebug("No Authentication Context for ServiceClient, fallback : DAPR");
            }
        }

        //just trying this for the mo
        public async Task<string> GetBearer()
        {
            var accessTokenResult = await _accessTokenProvider!.RequestAccessToken();
            if (accessTokenResult != null)
            {
                accessTokenResult.TryGetToken(out var token);
                return token!.Value;
            }

            if (_userAuthentication?.AuthCache?.AccessToken! != null)
            {
                return _userAuthentication?.AuthCache?.AccessToken!;
            }

            return _bearer;
        }

        public async Task<JsonDocument> GetJsonAsync(string path = "")
        {
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            var response = await _httpClient.GetAsync(callPath);
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JsonDocument>(str)!;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                try
                {
                    await HandleError("HTTP Get Failed", response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    await HandleError("HTTP Get Failed", response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return null!;
        }

        public async Task<Stream> GetStream(string path = "")
        {
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            var response = await _httpClient.GetAsync(callPath);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStream();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                try
                {
                    await HandleError("HTTP Get Failed", response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    await HandleError("HTTP Get Failed", response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return null!;
        }
        public async Task<JsonDocument> GetJsonAsyncOData(string path = "")
        {
            var callPath = GetCallPathOData(path);
            await SetBearerAsync();

            var response = await _httpClient.GetAsync(callPath);
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<JsonDocument>(str)!;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    try
                    {
                        await HandleError("HTTP Post Failed", response);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Connection failed {0}:{1}", response.StatusCode, response.ReasonPhrase);
                    try
                    {
                        await HandleError("HTTP Post Failed", response);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null!;
        }

        public async Task<string> PostJsonAsync(string path, JsonDocument callData)
        {
            return await PostJsonAsync(path, callData.RootElement.GetRawText());
        }

        public async Task<string> PostJsonAsync(string path, string callData)
        {
            // think we can do better handling etc but for the moment this will do
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            try
            {
                var response = await _httpClient.PostAsync(callPath, callData);
                
                await HandleError("HTTP Post Failed", response);

                var res = await response.Content.ReadAsStringAsync();

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<string> PostJsonAsync(string path, MultipartFormDataContent formData)
        {
            // think we can do better handling etc but for the moment this will do
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            try
            {
                var response = await _httpClient.PostAsync(callPath, formData);
                await HandleError("HTTP Post Failed", response);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<string> PatchJsonAsync(string path, string callData)
        {
            // think we can do better handling etc but for the moment this will do
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            try
            {
                var response = await _httpClient.PatchAsync(callPath, callData);

                await HandleError("HTTP Patch Failed", response);

                var res = await response.Content.ReadAsStringAsync();

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public async Task<HttpResponseMessage> PutJsonAsync(string path, JsonDocument callData)
        {
            return await PutJsonAsync(path, callData.RootElement.GetRawText());
        }

        public async Task<HttpResponseMessage> PutJsonAsync(string path, string callData)
        {
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            try
            {
                var response = await _httpClient.PutAsync(callPath, callData);
                await HandleError("HTTP Put Failed", response);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public async Task<HttpResponseMessage> Delete(string path)
        {
            var callPath = GetCallPath(path);
            await SetBearerAsync();

            try
            {
                var response = await _httpClient.DeleteAsync(callPath);
                await HandleError("HTTP Delete Failed", response);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task HandleError(string logMessage, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errtxt = await response.Content.ReadAsStringAsync();
                Dictionary<string, object?>? problemDetail = null;
                if (!string.IsNullOrWhiteSpace(errtxt) && errtxt.StartsWith('{'))
                {
                    problemDetail = JsonSerializer.Deserialize<Dictionary<string, object?>>(errtxt);
                }

                string? traceId = null;
                string? detail = null;
                if (problemDetail != null)
                {
                    if (problemDetail.TryGetValue("traceId", out object? tvalue))
                        traceId = tvalue?.ToString();
                    if (problemDetail.TryGetValue("detail", out object? dvalue))
                        detail = dvalue?.ToString();
                }

                Console.WriteLine("{0} : {1} : {2}", logMessage, response.StatusCode, errtxt);
                throw new HttpStatusException(response.StatusCode, $"{response.ReasonPhrase}: {detail}", traceId);
            }
        }

        private string GetCallPath(string path)
        {
            var callPath = _baseServiceUrl;

            if (!string.IsNullOrWhiteSpace(path))
            {
                callPath = _baseServiceUrl + path;
            }

            return callPath;
        }

        private string GetCallPathOData(string path)
        {
            var callPath = _baseServiceUrlOData;

            if (!string.IsNullOrWhiteSpace(path))
            {
                callPath = _baseServiceUrlOData + path;
            }

            return callPath;
        }

        internal class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }
    }
}
