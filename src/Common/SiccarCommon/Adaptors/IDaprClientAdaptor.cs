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
