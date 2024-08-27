using Dapr.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Siccar.Common.Adaptors
{
    public class DaprClientAdaptor : IDaprClientAdaptor
    {
        private readonly DaprClient _client;
        private readonly ILogger<DaprClientAdaptor> _logger;

        public DaprClientAdaptor(DaprClient client, ILogger<DaprClientAdaptor> logger)
        {
            _client = client;
            _logger = logger;
        }

        public Task<TValue> GetStateAsync<TValue>(string storeName, string key)
        {
            return _client.GetStateAsync<TValue>(storeName, key);
        }

        public Task<IReadOnlyList<BulkStateItem>> GetBulkStateAsync(string storeName, IReadOnlyList<string> keys, int? parallelism)
        {
            return _client.GetBulkStateAsync(storeName, keys, parallelism);
        }
        public Task SaveStateAsync<TValue>(string storeName, string key, TValue value)
        {
            return _client.SaveStateAsync(storeName, key, value);
        }

        public Task DeleteStateAsync(string storeName, string key)
        {
            return _client.DeleteStateAsync(storeName, key);
        }

        public async Task<Dictionary<string, string>> GetSecretsAsync(string secretStoreName)
        {
            var secretStores = await _client.GetBulkSecretAsync(secretStoreName);

            foreach (var s in secretStores)
            {
                _logger.LogDebug(">> Secret Stores {0} ", s.Key);
            }

            Dictionary<string, string> localSecretStore = new Dictionary<string, string>();

            if (secretStores.ContainsKey(secretStoreName))
            {
                // we are in the cloud
                _logger.LogDebug(">>> Using Cloud Config ");
                localSecretStore = secretStores[secretStoreName];
            }
            else
            {
                foreach (var s in secretStores)
                {
                    // local files
                    _logger.LogDebug(">>> Using Local Config ");
                    localSecretStore.Add(s.Key, secretStores[s.Key].First().Value);
                }
            }
            return localSecretStore;
        }

        public Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data)
        {
            return _client.PublishEventAsync(pubsubName, topicName, data);
        }

        public async Task<TResponse> InvokeMethodAsync<TResponse>(HttpMethod method, string appId, string methodName)
        {
            return await _client.InvokeMethodAsync<TResponse>(method, appId, methodName);
        }

        public HttpClient CreateInvokeHttpClient()
        {
            return DaprClient.CreateInvokeHttpClient();
        }

        public async Task InvokeBindingAsync<TRequest>(string bindingName, string operation, TRequest data, Dictionary<string, string> metadata)
        {
            await _client.InvokeBindingAsync(bindingName, operation, data, metadata);
        }
    }
}
