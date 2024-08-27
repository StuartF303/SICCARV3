using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Microsoft.Extensions.Logging;
using JetBrains.Annotations;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor.Notifications;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class ClientList
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }
        [Inject]
        NavigationManager navManager { get; set; }

        [Parameter]
        public string TenantId { get; set; }

        [CanBeNull]
        private Common.ServiceClients.Models.Tenant.Client _selectedClientToDelete;

        private List<Common.ServiceClients.Models.Tenant.Client> _clients = new();
        private bool _showDialog;
        private bool _preventSelection = true;
        private void RowSelecting(RowSelectingEventArgs<Common.ServiceClients.Models.Tenant.Client> args) { }
        private SfToast _toastNotification;

        protected override async Task OnInitializedAsync()
        {
            await ListClients();
            pageHistoryState.AddPageToHistory(navManager.Uri);
        }

        private async Task ListClients()
        {
            try
            {
                _clients = await TenantServiceClient.ListClients(TenantId);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error listing clients");
                Error?.ProcessError(e);
            }
        }

        private void RowSelectedHandler(RowSelectEventArgs<Common.ServiceClients.Models.Tenant.Client> selectedClient)
        {
            if (_preventSelection)
            {
                NavigationManager.NavigateTo($"tenants/{TenantId}/clients/{selectedClient.Data.ClientId}");
            }
            else
            {
                _preventSelection = true;
            }
        }

        private void AddClient()
        {
            NavigationManager.NavigateTo($"tenants/{TenantId}/clients/new");
        }

        private void DeleteClient()
        {
            _preventSelection = false;
            _showDialog = true;
        }

        private async Task ConfirmDeleteClient()
        {
            try
            {
                await TenantServiceClient.DeleteClient(TenantId, _selectedClientToDelete!.ClientId);
                await _toastNotification.ShowAsync(new ToastModel
                    { Title = "Success", Content = "Successfully deleted client", CssClass = "e-toast-success" });
                _selectedClientToDelete = null;
                await ListClients();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to delete client");
                Error?.ProcessError(e);
            }
            finally
            {
                _selectedClientToDelete = null;
            }
        }
    }
}