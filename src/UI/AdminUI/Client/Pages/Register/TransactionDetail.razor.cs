using System;
using System.Linq;
using Siccar.UI.Admin.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Siccar.Common.ServiceClients;
using System.Threading.Tasks;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Register
{
    public partial class TransactionDetail
    {
        [CascadingParameter] 
        public Error Error { get; set; }
        [Inject]
        NavigationManager NavManager { get; set; }
        [Parameter]
        public string RegisterId { get; set; }
        [Parameter]
        public string selectedTransactionTxId { get; set; }
        [Inject]
        IRegisterServiceClient registerServiceClient { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }

        public string _myTxt = "No data found";
        public StringBuilder myTextSB = new StringBuilder();
        private int textareaRows = 5;
        public TxModel transactionModel = new TxModel();
        protected string MyText
        {
            get => _myTxt;
            set
            {
                _myTxt = value;
                CalculateSize(value);
            }
        }

        protected override Task OnInitializedAsync()
        {
            pageHistoryState.AddPageToHistory(NavManager.Uri);
            return base.OnInitializedAsync();
        }
        private bool AccessiblePrevTransaction()
        {
            return transactionModel.PrevTransactionId != "0000000000000000000000000000000000000000000000000000000000000000";
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var transaction = await registerServiceClient.GetTransactionById(RegisterId, selectedTransactionTxId);

                var jsonSettings = new JsonSerializerOptions { WriteIndented = true };
                transactionModel.TransactionId = transaction.Id;
                transactionModel.PrevTransactionId = transaction.PrevTxId;
                transactionModel.Submitted = transaction.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                transactionModel.DocketId = transaction.Id;
                transactionModel.Validated = transaction.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"); //TODO Same as submitted for now
                transactionModel.SourceAddress = transaction.SenderWallet;
                transactionModel.TransactionSize = transaction.Payloads.Sum(p=> (long)p.PayloadSize).ToString();
                transactionModel.NumberOfPayloads = transaction.PayloadCount.ToString();
                transactionModel.MetaDataList = JsonSerializer.Serialize(transaction.MetaData, jsonSettings);
                transactionModel.PayloadList = JsonSerializer.Serialize(transaction.Payloads, jsonSettings);
                var destinationAddresses = "";
                foreach (var recipientWallet in transaction.RecipientsWallets)
                {
                    destinationAddresses += recipientWallet + Environment.NewLine;
                }
                transactionModel.DestinationAddresses = destinationAddresses;
            }
            catch (Exception err)
            {
                Error?.ProcessError(err);
            }
        }

        private void CalculateSize(string value)
        {
            textareaRows = Math.Max(value.Split('\n').Length, value.Split('\r').Length);
            textareaRows = Math.Max(textareaRows, 2);
            if (textareaRows > 5)
            {
                textareaRows = 5;
            }
        }
        private void PreviousTxClickHandler()
        {
            NavManager.NavigateTo($"{RegisterId}/transactions/{transactionModel.PrevTransactionId}/");
        }
    }
}