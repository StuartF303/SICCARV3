using Bunit;
using Siccar.Platform;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using Xunit;

namespace AdminUiTest.Pages.Wallet
{
    public class WalletMnemonicTest : TestContext
    {
        public WalletMnemonicTest()
        {
            Services.AddSyncfusionBlazor();
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [Fact]
        public void Should_Copy_Wallet_Mnemonic_To_Clipboard()
        {
            var component = RenderComponent<WalletMnemonic>(parameters => parameters.Add(p => p.Wallet,
                new CreateWalletResponse(null, "test wallet mnemonic")));

            var button = component.Find("button");
            button.Click();
            Assert.Contains(JSInterop.Invocations, i => i.Identifier == "navigator.clipboard.writeText" && i.Arguments[0]!.ToString() == "test wallet mnemonic");
        }
    }
}
