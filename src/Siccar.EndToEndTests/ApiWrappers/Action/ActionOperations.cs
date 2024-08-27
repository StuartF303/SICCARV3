using System.Net.Http.Json;
using Siccar.EndToEndTests.Blueprint.Models;
using Siccar.EndToEndTests.Common;

namespace Siccar.EndToEndTests.Action
{
    internal class ActionOperations
    {
        private readonly string _actionUri = "api/actions";

        public async Task<HttpResponseMessage> Submit(EndToEndTestFixture testFixture, string previousTransactionId, string blueprintId, 
            string walletAddress,
            string registerId, 
            dynamic data)
        {
            var httpClient = await HttpClientFactory.Create();
            
            var response = await httpClient.PostAsJsonAsync(_actionUri, new
            {
                previousTxId = previousTransactionId,
                blueprintId,
                walletAddress,
                registerId,
                data
            });
            
            await testFixture.WaitForPubSub("OnTransaction_Confirmed", "EndToEndTests_TransactionConfirmed",
                () => { return Task.CompletedTask; }, 
                ("OnTransaction_Confirmed", (await response.GetContent<Transaction>())!.TxId, 1)!);
            
            return response;
        }
    }
}
