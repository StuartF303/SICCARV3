using System.Net.Http.Json;
using Siccar.Common.Exceptions;
using Siccar.EndToEndTests.Common;

namespace Siccar.EndToEndTests.Blueprint
{
    public class BlueprintOperations
    {
        private readonly string _blueprintUri = "api/blueprints";
        
        public async Task<HttpResponseMessage> Create(Models.Blueprint blueprint)
        {
            var httpClient = await HttpClientFactory.Create();
            var result = await httpClient.PostAsJsonAsync(_blueprintUri, blueprint);
            return result;
        }

        public async Task<HttpResponseMessage> Publish(Models.Blueprint blueprint, string walletAddress, string registerId)
        {
            var httpClient = await HttpClientFactory.Create();
            var result = await httpClient.PostAsJsonAsync($"{_blueprintUri}/{walletAddress}/{registerId}/publish", blueprint);
            return result;
        }

        public async Task<HttpResponseMessage> CreateAndPublish(EndToEndTestFixture testFixture, Models.Blueprint blueprint, string walletAddress, string registerId)
        {
            HttpResponseMessage? publishResponse = null;
            var createResponse = await Create(blueprint);
            var stringres = await createResponse.Content.ReadAsStringAsync();
            if (!createResponse.IsSuccessStatusCode)
            {
                throw new HttpStatusException(createResponse.StatusCode, stringres);
            }
            await testFixture.WaitForPubSub("OnTransaction_Confirmed", "EndToEndTests_TransactionConfirmed", async () =>
            {
                publishResponse = await Publish(blueprint, walletAddress, registerId);
            }, ("OnTransaction_Confirmed", 1));

            return publishResponse!;
        }
    }
}
