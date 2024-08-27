using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.UI.Admin.Models;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ParticipantsList
    {
        [CascadingParameter]
        public Error Error { get; set; }
        private List<Register> _registers;
        [Parameter]
        public string RegisterId { get; set; } = "noregister";
        private List<ParticipantModel> Participants { get; set; }
        private string _ddListValue { get; set; }

        public SfDataManager dm { get; set; }
        public bool SpinnerVisible { get; set; }
        public string[] pageDropdown { get; set; } = new[] { "All", "5", "10", "15", "20" };
        public SfGrid<ParticipantModel> _grid;
        public Query Query;

        [Inject]
        IRegisterServiceClient RegisterServiceClient { get; set; }
        [Inject]
        NavigationManager NavManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _registers = await RegisterServiceClient.GetRegisters();
                await RegisterServiceClient.StartEvents();
                RegisterServiceClient.OnTransactionValidationCompleted += OnTransactionValidationCompleted;
                foreach (var item in _registers)
                {
                    await RegisterServiceClient.SubscribeRegister(item.Id);
                }

                Query = new Query().AddParams("RegisterId", RegisterId.IsNullOrEmpty() ? "noregister" : RegisterId);
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {   
                if (!RegisterId.IsNullOrEmpty() && RegisterId != "noregister")
                {
                    _ddListValue = RegisterId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            await base.OnParametersSetAsync();
        }

        private async Task OnRegisterChangeHandler(ChangeEventArgs<string, Register> args)
        {
            RegisterId = args.ItemData.Id;
            _ddListValue = RegisterId;
            Query.Queries.Params.Remove("RegisterId");
            Query.AddParams("RegisterId", RegisterId.IsNullOrEmpty() ? "noregister" : RegisterId);

            await JsRuntime.InvokeVoidAsync("ChangeUrl", $"/participantslist/{RegisterId}");
            await _grid.Refresh();
            StateHasChanged();
        }

        private void RowSelectedHandler(RowSelectEventArgs<ParticipantModel> selectedParticipant)
        {
            NavManager.NavigateTo($"participant/{RegisterId}/{selectedParticipant.Data.Id}");
        }

        private async Task OnTransactionValidationCompleted(TransactionConfirmed transactionConfirmed)
        {
            if (transactionConfirmed.MetaData.TransactionType == TransactionTypes.Participant
                && transactionConfirmed.MetaData.RegisterId == _ddListValue)
            {
                transactionConfirmed.MetaData.RegisterId = _ddListValue;
                await _grid.Refresh();
                StateHasChanged();
            }
        }

        private void AddParticipant()
        {
            NavManager.NavigateTo($"participant/{RegisterId}/new");
        }
    }
}
