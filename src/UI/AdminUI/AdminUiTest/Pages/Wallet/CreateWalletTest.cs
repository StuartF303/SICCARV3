using System.Linq;
using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using Xunit;

namespace AdminUiTest.Pages.Wallet
{
    public class CreateWalletTest : TestContext
    {
        private readonly IWalletServiceClient _fakeWalletServiceClient;

        public CreateWalletTest()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            Services.AddSingleton(_fakeWalletServiceClient);

            Services.AddSyncfusionBlazor();
        }

        [Fact]
        public void Click_Create_Wallet_With_Valid_Wallet_Should_Call_Create_Wallet()
        {
            var createWallet = RenderComponent<CreateWallet>();
            createWallet.Instance.Wallet.Name = "Test-Wallet";
            createWallet.Instance.Wallet.Description = "A Test Wallet";

            var saveButton = createWallet.FindAll("button").First(b => b.TextContent.Contains("Create Wallet"));
            saveButton.Click();

            A.CallTo(() => _fakeWalletServiceClient.CreateWallet(createWallet.Instance.Wallet.Name,
                createWallet.Instance.Wallet.Description, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Click_Create_Wallet_With_Invalid_Wallet_Should_Not_Call_Create_Wallet()
        {
            var createWallet = RenderComponent<CreateWallet>();
            createWallet.Instance.Wallet.Name = "";

            var saveButton = createWallet.FindAll("button").First(b => b.TextContent.Contains("Create Wallet"));
            saveButton.Click();

            A.CallTo(() => _fakeWalletServiceClient.CreateWallet(A<string>.Ignored,
                A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Valid_Wallet_Save_Should_Show_Mnemonic()
        {
            var createWallet = RenderComponent<CreateWallet>();
            createWallet.Instance.Wallet.Name = "Test-Wallet";
            createWallet.Instance.Wallet.Description = "A Test Wallet";

            var saveButton = createWallet.FindAll("button").First(b => b.TextContent.Contains("Create Wallet"));
            saveButton.Click();
            Assert.True(createWallet.HasComponent<WalletMnemonic>());
        }

        [Fact]
        public void Before_Save_Wallet_Mnemonic_Should_Not_Be_Shown()
        {
            var createWallet = RenderComponent<CreateWallet>();
            Assert.False(createWallet.HasComponent<WalletMnemonic>());
        }
    }
}
