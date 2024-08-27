using System.Net;
using WalletService.IntegrationTests.Config;
using WalletService.IntegrationTests.Models;
using WalletService.IntegrationTests.TestData;

namespace WalletService.IntegrationTests
{
    public class GetWalletTests : WalletTestBase, IClassFixture<WalletWebApplicationFactory<WebHostStartup>>
    {
        public GetWalletTests(WalletWebApplicationFactory<WebHostStartup> factory) : base(factory, true, true)
        {
        }

        [Fact]
        public async Task GetWallet_WalletExists_ShouldReturnValidWallet()
        {
            // Arrange
            await WalletTestData.Clear();
            var createdWallet = await WalletTestData.CreateNewDefault(WalletOperations);

            // Act
            var retrievedWalletResponse = await WalletOperations.Get(createdWallet!.Address!);
            var retrievedWallet = await retrievedWalletResponse.Content.ReadFromJsonAsync<Wallet>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, retrievedWalletResponse.StatusCode);
            WalletAssertions.IsValid(retrievedWallet, createdWallet.Name, createdWallet.Owner,
                createdWallet.Tenant);
        }
    }
}
