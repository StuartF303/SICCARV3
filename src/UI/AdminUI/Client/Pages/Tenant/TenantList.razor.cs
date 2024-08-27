using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Data;
using System.Linq;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Models;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class TenantList
    {
        [CascadingParameter]
        public Error Error { get; set; }
        public SfDataManager dm { get; set; }
        public bool SpinnerVisible { get; set; }
        public string[] pageDropdown { get; set; } = new[] { "All", "5", "10", "15", "20" };
        public List<TenantModel> Tenants { get; set; }
        public SfGrid<TenantModel> Grid;

        [Inject]
        ITenantServiceClient TenantServiceClient { get; set; }
        [Inject]
        NavigationManager NavManager { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var tenants = await TenantServiceClient.All();
                CreateListOfTenantModels(tenants);
                pageHistoryState.AddPageToHistory(NavManager.Uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Error?.ProcessError(e);
            }
        }

        private void CreateListOfTenantModels(List<Platform.Tenant> tenants)
        {
            var tenantViewModels = tenants.Select(tenant =>
            {
                var newTenant = new TenantModel
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Authority = tenant.Authority,
                    Admins = tenant.Admins.Count(),
                    Clients = tenant.Clients.Count(),
                    Registers = tenant.Registers.Count(),
                    AccountsCount = tenant.AccountsCount,
                };
                return newTenant;
            });

            Tenants = tenantViewModels.ToList();
        }

        private void RowSelectedHandler(RowSelectEventArgs<TenantModel> selectedTenant)
        {
            NavManager.NavigateTo($"tenants/{selectedTenant.Data.Id}/");
        }
    }
}