using System.Net;
using WalletService.IntegrationTests.Config;
using WalletService.IntegrationTests.Models;
using WalletService.IntegrationTests.TestData;

namespace WalletService.IntegrationTests
{
    public class CreateWalletTests : WalletTestBase, IClassFixture<WalletWebApplicationFactory<WebHostStartup>>
    {
        public CreateWalletTests(WalletWebApplicationFactory<WebHostStartup> factory) : base(factory, true, true)
        {

        }

        [Fact]
        public async Task CreateWallet_ValidWallet_ShouldSaveWallet()
        {
            // Arrange
            await WalletTestData.Clear();
            var wallet = WalletTestData.NewDefault();

            // Act
            var response = await WalletOperations.Create(wallet);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            var createdWallet = await response.Content.ReadFromJsonAsync<CreateWalletResponse>();
            await WalletAssertions.HasCount(wallet.Name!, 1);
            WalletAssertions.IsValid(createdWallet, createdWallet!.Name, createdWallet.Owner,
                createdWallet.Tenant);
            PubSubMessagesWereReceived(("OnWallet_AddressCreated", 1, createdWallet.Address!));
        }
    }
}
