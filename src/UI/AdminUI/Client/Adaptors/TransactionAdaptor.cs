using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
using System.Transactions;
using System.Collections.Generic;
using Siccar.Platform;
using System.Linq;
using Humanizer;
using Siccar.UI.Admin.Models;

namespace Siccar.UI.Admin.Adaptors
{
    public class TransactionAdaptor : DataAdaptor
    {
        private readonly IRegisterServiceClient _registerServiceClient;

        public TransactionAdaptor()
        {
        }

        public TransactionAdaptor(IRegisterServiceClient registerServiceClient)
        {
            _registerServiceClient = registerServiceClient;
        }

        public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            var registerId = dataManagerRequest.Params.First().Value.ToString();
            var sort = dataManagerRequest.Sorted.First();
            var direction = sort.Direction == "ascending" ? "asc" : "desc";

            List<TransactionModel> result =
                await _registerServiceClient.GetAllTransactions(registerId,
                    $"?$orderby={sort.Name} {direction}  & skip={dataManagerRequest.Skip}&top={dataManagerRequest.Take}");

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

            var register = await _registerServiceClient.GetRegister(registerId);
            DataResult dataResult = new DataResult()
            {
                Result = results,
                Count = (int)register.Height
            };

            return dataResult;
        }
    }
 
}
