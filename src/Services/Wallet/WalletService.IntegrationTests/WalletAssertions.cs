using WalletService.IntegrationTests.Models;
using WalletService.IntegrationTests.TestData;

namespace WalletService.IntegrationTests
{
    public class WalletAssertions
    {
        public static async Task HasCount(string walletName, int expectedCount)
        {
            var actualCount = await WalletTestData.MySqlRepository!.GetCount(walletName);
            Assert.Equal(expectedCount, actualCount);
        }

        public static void IsValid(CreateWalletResponse? actualWallet,
            string? expectedName,
            string? expectedOwner,
            string? expectedTenant)
        {
            Assert.NotNull(actualWallet);
            Assert.NotNull(actualWallet?.Mnemonic);
            Assert.NotNull(actualWallet);
            Assert.NotNull(actualWallet!.Address);
            Assert.Equal(expectedName, actualWallet.Name);
            Assert.Equal(expectedOwner, actualWallet.Owner);
            Assert.Equal(expectedTenant, actualWallet.Tenant);
        }

        public static void IsValid(Wallet? actualWallet,
            string? expectedName,
            string? expectedOwner,
            string? expectedTenant)
        {
            Assert.NotNull(actualWallet);
            Assert.NotNull(actualWallet!.Address);
            Assert.Equal(expectedName, actualWallet.Name);
            Assert.Equal(expectedOwner, actualWallet.Owner);
            Assert.Equal(expectedTenant, actualWallet.Tenant);
        }

    }
}
