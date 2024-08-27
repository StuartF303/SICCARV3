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
