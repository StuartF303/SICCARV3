using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using JetBrains.Annotations;
using System.Collections.Generic;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class ClientDetail
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }
        [Inject]
        NavigationManager navManager { get; set; }

        [Parameter]
        public string TenantId { get; set; }

        [Parameter]
        [CanBeNull]
        public string ClientId { get; set; }

        private Common.ServiceClients.Models.Tenant.Client _client;

        protected override Task OnInitializedAsync()
        {
            pageHistoryState.AddPageToHistory(navManager.Uri);
            return base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(ClientId) && ClientId.ToLower() != "new")
            {
                try
                {
                    _client = await TenantServiceClient.Get(TenantId, ClientId);
                }
                catch (Exception e)
                {
                    Error?.ProcessError(e);
                }
            }
        }
    }
}