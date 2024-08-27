#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Shared.Components;
using System.Linq;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class TenantWalletsList
    {
        private List<WalletModel> _wallets = new();
        private SfGrid<WalletModel>? _grid;

        [CascadingParameter]
        public Error? Error { get; set; }

        [Inject]
        IUserServiceClient? UserServiceClient { get; set; }

        [Inject]
        IWalletServiceClient? WalletServiceClient { get; set; }

        [Inject]
        Services.PageHistoryState? pageHistoryState { get; set; }

        [Inject]
        NavigationManager? NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetWallets();
            pageHistoryState!.AddPageToHistory(NavigationManager!.Uri);
        }

        private async Task GetWallets()
        {
            try
            {
                if (UserServiceClient is null) return;

                var users = await UserServiceClient.All();

                var userModels = users.Select(u => new UserModel
                {
                    Id = u.Id.ToString(),
                    Name = u.UserName,
                });

                var platformWallets = (await WalletServiceClient!.ListWallets(allInTenant: true)) ?? new();

                _wallets = platformWallets.Select(pw => new WalletModel()
                {
                    Name = pw.Name ?? "",
                    Address = pw.Address ?? "",
                    OwnerName = userModels.FirstOrDefault(_ => _.Id == pw.Owner)?.Name ?? "",
                }).ToList();

                if (_grid != null)
                {
                    await _grid.Refresh();
                }
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
        }

        private void OnWalletCellSelecting(CellSelectingEventArgs<WalletModel> args)
        {
            if (args.CellIndex > 2)
            {
                // Prevent navigation when clicking delete
                args.Cancel = true;
            }
            else
            {
                NavigationManager!.NavigateTo($"wallets/{args.Data.Address}");
            }
        }

        public class WalletModel
        {
            public string Name { get; init; } = "";

            public string Address { get; init; } = "";

            public string OwnerName { get; init; } = "";
        }

        public class UserModel
        {
            public string Id { get; init; } = "";

            public string? Name { get; init; } = "";
        }
    }
}