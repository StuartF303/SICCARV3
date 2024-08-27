using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
using System.Collections.Generic;
using Siccar.Platform;
using System.Linq;
using Siccar.UI.Admin.Models;

namespace Siccar.UI.Admin.Adaptors
{
    public class TransactionAdaptorOData : DataAdaptor
    {
        private readonly IRegisterServiceClient _registerServiceClient;

        public TransactionAdaptorOData(IRegisterServiceClient registerServiceClient)
        {
            _registerServiceClient = registerServiceClient;
        }

        public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            var registerId = dataManagerRequest.Params.First().Value.ToString();
            var sort = dataManagerRequest.Sorted.First();
            var direction = sort.Direction == "ascending" ? "asc" : "desc";

            var resultOData =
                await _registerServiceClient.GetAllTransactionsOData(registerId,
                    $"?$orderby={sort.Name} {direction}  & skip={dataManagerRequest.Skip}&top={dataManagerRequest.Take} &$count=true");

            List<TransactionModel> result =
                resultOData.Value;

            var results = result.Select(r => new RegisterDetailRawTransaction
            {
                TxId = r.TxId,
                SenderWallet = r.SenderWallet,
                TimeStamp = r.TimeStamp
            }).ToList();

            if (results.Any())
            {
                results[0].IsFirstRow = true;
            }

            DataResult dataResult = new()
            {
                Result = results,
                Count = resultOData.MetaCount
            };
            return dataResult;
        }
    }
}
