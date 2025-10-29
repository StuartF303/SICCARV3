// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Siccar.Common.Adaptors
{
    public interface IDaprClientAdaptor
    {
        Task<TValue> GetStateAsync<TValue>(string storeName, string key);
        Task<IReadOnlyList<BulkStateItem>> GetBulkStateAsync(string storeName, IReadOnlyList<string> keys, int? parallelism);
        Task SaveStateAsync<TValue>(string storeName, string key, TValue value);
        Task DeleteStateAsync(string storeName, string key);
        Task<Dictionary<string, string>> GetSecretsAsync(string secretsStoreName);
        Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data);
        Task<TResponse> InvokeMethodAsync<TResponse>(HttpMethod method, string appId, string methodName);
        HttpClient CreateInvokeHttpClient();
        public Task InvokeBindingAsync<TRequest>(string bindingName, string operation, TRequest data, Dictionary<string, string> metadata);
    }
}
