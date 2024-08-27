using Siccar.Common;
using System.Net;
using WalletService.IntegrationTests.Config;
using WalletService.IntegrationTests.Models;

namespace WalletService.IntegrationTests
{
    public class AuthorisationTests : WalletTestBase, IClassFixture<WalletWebApplicationFactory<WebHostStartup>>
    {
        public AuthorisationTests(WalletWebApplicationFactory<WebHostStartup> factory) : base(factory, false, false)
        {
        }

        [Fact]
        public async Task GetWallet_UserNotInRole_ShouldReturn403Forbidden()
        {
           var response = await WalletOperations.Get("", false);
           Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ListAll_UserNotInWaleltUserRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.ListAll(Constants.WalletDelegateRole, allInTenant: false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(new object[] { Constants.BlueprintAdminRole })]
        [InlineData(new object[] { Constants.BlueprintAuthoriserRole })]
        [InlineData(new object[] { Constants.InstallationBillingRole })]
        [InlineData(new object[] { Constants.RegisterCreatorRole })]
        [InlineData(new object[] { Constants.RegisterMaintainerRole })]
        [InlineData(new object[] { Constants.RegisterReaderRole })]
        [InlineData(new object[] { Constants.TenantBillingRole })]
        [InlineData(new object[] { Constants.TenantAppAdminRole })]
        [InlineData(new object[] { Constants.WalletUserRole })]
        [InlineData(new object[] { Constants.WalletOwnerRole })]
        [InlineData(new object[] { Constants.WalletDelegateRole })]
        public async Task ListAllInTenant_UserNotInRoleAndAllInTenantIsTrue_ShouldReturn403Forbidden(string role)
        {
            var response = await WalletOperations.ListAll(role, allInTenant: true);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ListAllInTenant_UserIsInWalletUserRoleDelegateAndAllInTenantIsFalse_ShouldReturn200Ok()
        {
            var response = await WalletOperations.ListAll(Constants.WalletUserRole, allInTenant: false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(new object[] { Constants.InstallationAdminRole })]
        [InlineData(new object[] { Constants.InstallationReaderRole })]
        [InlineData(new object[] { Constants.TenantAdminRole })]
        public async Task ListAllInTenant_UserIsRequiredRollAndAllInTenantIsTrue_ShouldReturn200Ok(string role)
        {
            var response = await WalletOperations.ListAll(role, allInTenant: true);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateWallet_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.Create(TestData.WalletTestData.NewDefault(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateWallet_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.Update("test-address", new UpdateWallet(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteWallet_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.Delete("test-address", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTransactions_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.GetTransactions("test-address", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTransaction_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.GetTransaction("test-address", "test-id", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateTransaction_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.CreateTransaction("test-address", new CreateTransaction(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DecryptTransactionPayloads_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.DecryptTransactionPayloads("test-address", new {}, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Validate_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.Validate("test-address", new { }, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetDelegates_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.GetDelegates("test-address", new { }, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddDelegate_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.AddDelegate("test-address", new WalletAccess(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task RemoveDelegate_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.RemoveDelegate("test-address", "test-subject", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDelegate_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await WalletOperations.UpdateDelegate("test-address", new WalletAccess(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
