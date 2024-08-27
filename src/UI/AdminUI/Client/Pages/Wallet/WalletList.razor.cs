#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Wallet
{
    public partial class WalletList
    {
        private List<WalletModel> _wallets = new();
        private Platform.Wallet? _walletToDelete;
        private SfGrid<WalletModel>? _grid;

        [CascadingParameter]
        public Error? Error { get; set; }
        [Inject]
        IWalletServiceClient? WalletServiceClient { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState? pageHistoryState { get; set; }
        [Inject]
        NavigationManager? NavigationManager { get; set; }

        public bool ShowCreateWalletDialog { get; set; }

        public bool ShowDeleteWalletDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetWallets();
            pageHistoryState!.AddPageToHistory(NavigationManager!.Uri);
        }

        private async Task GetWallets()
        {
            try
            {
                var wallets = await WalletServiceClient!.ListWallets();
                var walletModels = wallets!.Select(async w => new WalletModel
                {
                    Wallet = w,
                    PendingTransactionCount = (await WalletServiceClient.GetWalletTransactions(w.Address!)).Count
                }).ToList();

                _wallets = (await Task.WhenAll(walletModels)).ToList();
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

        private void CreateWallet()
        {
            ShowCreateWalletDialog = true;
        }

        private void OpenDeleteWalletDialog(Platform.Wallet wallet)
        {
            ShowDeleteWalletDialog = true;
            _walletToDelete = wallet;
        }

        private async Task ConfirmDeleteWallet()
        {
            try
            {
                await WalletServiceClient!.DeleteWallet(_walletToDelete!.Address!);
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
            finally
            {
                _walletToDelete = null;
                ShowDeleteWalletDialog = false;
                await GetWallets();
            }
        }

        private void CancelDeleteWallet()
        {
            _walletToDelete = null;
            ShowDeleteWalletDialog = false;
        }

        private async Task CreateWalletVisibleChanged(bool visible)
        {
            if (!visible && ShowCreateWalletDialog)
            {
                ShowCreateWalletDialog = false;
                await GetWallets();
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
                NavigationManager!.NavigateTo($"wallets/{args.Data!.Wallet!.Address}");
            }
        }
    }

    public class WalletModel
    {
        public Platform.Wallet? Wallet { get; set; }
        public int PendingTransactionCount { get; set; }
    }
}