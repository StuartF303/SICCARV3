using System.Collections.Generic;
using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Pages.Wallet;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor;
using Xunit;

namespace AdminUiTest.Pages.Wallet
{
    public class WalletListTest : TestContext
    {
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly FakeNavigationManager _navigationManager;
        private readonly List<Siccar.Platform.Wallet> _wallets;

        public WalletListTest()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();

            _wallets = new List<Siccar.Platform.Wallet>
            {
                new()
                {
                    Name = "Test-wallet",
                    Address = "Test-Address"
                }
            };

            A.CallTo(() => _fakeWalletServiceClient.ListWallets(false)).Returns(_wallets);

            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            _navigationManager = Services.GetRequiredService<FakeNavigationManager>();
        }

        [Fact]
        public void Should_Load_Wallets_List()
        {
            RenderComponent<WalletList>();
            A.CallTo(() => _fakeWalletServiceClient.ListWallets(false)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Click_Client_Should_Navigate_To_Client_Detail_Page()
        {
            var walletList = RenderComponent<WalletList>();
            var walletRow = walletList.FindAll("td").FirstOrDefault(e => e.TextContent.Contains("Test-wallet"));
            Assert.NotNull(walletRow);
            walletRow.Click();
            Assert.Equal($"http://localhost/wallets/{_wallets.First().Address}", _navigationManager.Uri);
        }
    }
}
