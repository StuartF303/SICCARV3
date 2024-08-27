using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siccar.Platform;
using Siccar.Common.ServiceClients;
using Syncfusion.Blazor.Grids;
using System;
using Siccar.UI.Admin.Models;
using Syncfusion.Blazor.Data;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class RegisterDetail
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Parameter]
        public string RegisterId { get; set; }
        [Inject]
        NavigationManager NavManager { get; set; }

        [Inject]
        IRegisterServiceClient RegisterServiceClient { get; set; }

        string AnimationTitle { get; set; } = "newTrans";
        SfGrid<RegisterDetailRawTransaction> _grid;
        private Register _register;
        public Query Query;
        private string _statusStyle = "color:forestgreen; font-size: 20px;";
        
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                _register = await RegisterServiceClient.GetRegister(RegisterId);
                Query = new Query().AddParams("RegisterId", RegisterId);
                await RegisterServiceClient.StartEvents();
                RegisterServiceClient.OnTransactionValidationCompleted += OnTransactionValidationCompleted;
                await RegisterServiceClient.SubscribeRegister(RegisterId);
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
        }

        private async Task OnTransactionValidationCompleted(TransactionConfirmed transactionConfirmed)
        {
            try
            {
                _register = await RegisterServiceClient.GetRegister(RegisterId);
                await _grid.Refresh();

                StateHasChanged();
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
    
        }

        public int GetTransactionsPerDay()
        {
            return 50;
        }

        public async void RowBoundHandler(RowDataBoundEventArgs<RegisterDetailRawTransaction> args)
        {
            if (args.Data.IsFirstRow)
            {
                AnimationTitle = "newTrans";
                args.Row.AddClass(new[] { "newTransaction" });
                args.Row.AddStyle(new[] { $"animation-name: {AnimationTitle};  animation-duration: 3s;" });
                await SetAnimationName();
            }
        }

        private async Task SetAnimationName() {
            await Task.Delay(3000);
            AnimationTitle = "waiting";
            StateHasChanged();
        }

        public string GetStyle(string txId)
        {
            // Will need to put suitable logic in here
            if (txId == "abcd1234" || txId == "abcd1235")
            {
                return "color:lightGray; font-size: 30px; width: 50px; padding-left: 5px; padding-right:5px; background-color: white; text-align:center";
            }
            if (txId == "abcd1236")
            {
                return "color:black; font-size: 30px; width: 50px; padding-left: 5px; padding-right:5px; background-color: white; text-align:center";
            }
            else
            {
                return "color:white; font-size: 30px; width: 50px; padding-left: 5px; padding-right:5px; background-color: forestgreen; text-align:center";
            }
        }

        private void RowSelectedHandler(RowSelectEventArgs<RegisterDetailRawTransaction> selectedTransaction)
        {
            NavManager.NavigateTo($"{RegisterId}/transactions/{selectedTransaction.Data.TxId}/");
        }

    }

}
