using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor.Notifications;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class TenantInfo
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Inject]
        ITenantServiceClient TenantServiceClient { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }
        [Inject]
        NavigationManager navManager { get; set; }

        [Parameter]
        public string SelectedTenantId { get; set; }
        private Platform.Tenant SelectedTenant { get; set; }
        private EditForm Form { get; set; }
        private SfToast ToastNotification;

        protected override async Task OnInitializedAsync()
        {
            SelectedTenant = await TenantServiceClient.GetTenantById(SelectedTenantId);
            StateHasChanged();
            pageHistoryState.AddPageToHistory(navManager.Uri);
        }

        private async Task DataChangedCallbackHandler()
        {
            if (Form.EditContext.Validate())
            {
                try
                {
                    await TenantServiceClient.UpdateTenant(SelectedTenantId, SelectedTenant);
                    await ToastNotification.ShowAsync(new ToastModel { Title = "Success", Content = "Successfully updated Tenant", CssClass = "e-toast-success" });
                }
                catch (Exception e)
                {
                    Error?.ProcessError(e);
                }
            }
        }
    }
}