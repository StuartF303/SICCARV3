using Json.Logic.Rules;
using Json.Path;
using Siccar.Application;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ActionService.V1.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly IRegisterServiceClient _registerServiceClient;
        private readonly IPayloadResolver _payloadResolver;
        private readonly IWalletServiceClient _walletServiceClient;

        public CalculationService(IRegisterServiceClient registerServiceClient, IPayloadResolver payloadResolver, IWalletServiceClient walletServiceClient)
        {
            _registerServiceClient = registerServiceClient;
            _payloadResolver = payloadResolver;
            _walletServiceClient = walletServiceClient;
        }

        public async Task<JsonDocument> RunActionCalculationsAsync(Siccar.Application.Action currentAction, ActionSubmission actionSubmission, string instanceId)
        {
            var submittedDataWithCalculationResults = JsonSerializer.Deserialize<Dictionary<string, object>>(actionSubmission.Data.RootElement.GetRawText());

            if (currentAction.Calculations.Any())
            {
                var allPrevTransactions = await _registerServiceClient.GetTransactionsByInstanceId(actionSubmission.RegisterId, instanceId);
                var previousData = await _payloadResolver.GetAllPreviousPayloadsForWalletAsync(actionSubmission.WalletAddress, allPrevTransactions, _walletServiceClient);

                var submittedKvp = JsonSerializer.Deserialize<Dictionary<string, object>>(actionSubmission.Data.RootElement.GetRawText());
                previousData ??= new Dictionary<string, object>();

                //combine all data, updating the value if already been scanned  - tho this will depend on the locaing order ...
                submittedKvp.ToList().ForEach(x => { if (!previousData.TryAdd(x.Key, x.Value)) previousData[x.Key] = x.Value; });

               
                foreach (var kvp in currentAction.Calculations)
                {
                    var result = RunCalculation(kvp.Value, previousData);
                    submittedDataWithCalculationResults.Add(kvp.Key, result);

                    // Add the result to the list of all data so that additional calculations can be done on previous calculation results.
                    previousData.Add(kvp.Key, result);
                }
               
            }

            return JsonDocument.Parse(JsonSerializer.Serialize(submittedDataWithCalculationResults));
        }

        private static decimal RunCalculation(JsonNode calculation, Dictionary<string, object> allData)
        {
            try
            {
                Json.Logic.Rule lr = JsonSerializer.Deserialize<Json.Logic.Rule>(calculation.ToString());
                JsonNode data = JsonNode.Parse(JsonSerializer.Serialize(allData));
                var result = lr.Apply(data);
                return result.AsValue().GetValue<decimal>();
            }
            catch(Exception ex)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "Calculation Error: Could not obtain calculation result.", null, ex);
            }
        }
    }
}
