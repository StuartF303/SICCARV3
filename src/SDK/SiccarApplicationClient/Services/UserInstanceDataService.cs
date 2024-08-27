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

using Siccar.Common.ServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Siccar.Application.Client.Services
{
    public class UserInstanceDataService
    {
        private readonly IRegisterServiceClient _registerServiceClient;
        private readonly IWalletServiceClient _walletServiceClient;
        public UserInstanceDataService(IRegisterServiceClient registerService, IWalletServiceClient walletServiceClient)
        {
            _registerServiceClient = registerService;
            _walletServiceClient = walletServiceClient;
        }

        public async Task<JsonDocument> GetAllDataForUserByTxInstanceId(string transactionId, string walletAddress, string registerId, string accessToken = null)
        {
            var targetTx = await _registerServiceClient.GetTransactionById(registerId, transactionId);
            var allPrevTransactions = await _registerServiceClient.GetTransactionsByInstanceId(registerId, targetTx.MetaData.InstanceId);

            var previousTransactionData = new List<byte[]>();
            foreach (var transaction in allPrevTransactions)
            {
                var bytes = await _walletServiceClient.DecryptTransaction(transaction, walletAddress, accessToken);
                if (bytes != null)
                    previousTransactionData.AddRange(bytes);
            }

            var combinedKvpData = new Dictionary<string, object>();
            foreach (var bytes in previousTransactionData)
            {
                var payloadKvp = JsonSerializer.Deserialize<Dictionary<string, object>>(bytes, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                payloadKvp.ToList().ForEach(dataKVP => combinedKvpData[dataKVP.Key] = dataKVP.Value);
            }
            return JsonDocument.Parse(JsonSerializer.Serialize(combinedKvpData));
        }
    }
}
