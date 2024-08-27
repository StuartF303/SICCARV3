using System.Net;
using FakeItEasy;
using RegisterService.IntegrationTests.Config;
using Siccar.Common;
using Siccar.Platform;

namespace RegisterService.IntegrationTests
{
    public class AuthorisationTests : RegisterTestBase, IClassFixture<RegisterWebApplicationFactory<WebHostStartup>>
    {

        public AuthorisationTests(RegisterWebApplicationFactory<WebHostStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDockets_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetDockets("test-register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.WalletUserRole)]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetDockets_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await RegisterOperations.GetDockets("test-register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDocket_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetDocket("test-register", 1, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.WalletUserRole)]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetDocket_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await RegisterOperations.GetDocket("test-register", 1, role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTransactions_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetTransactions("test-register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.WalletUserRole)]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetTransactions_UserInRole_ShouldReturn200Ok(string role)
        {
            A.CallTo(() => _fakeTenantServiceClient!.GetTenantById(A<string>.Ignored)).Returns(new Tenant() { Registers = new() { "test-register" } });

            var response = await RegisterOperations.GetTransactions("test-register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTransaction_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetTransaction("test-register", "test-transaction", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.WalletUserRole)]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetTransaction_UserInRole_ShouldReturn200Ok(string role)
        {
            A.CallTo(() => _fakeTenantServiceClient!.GetTenantById(A<string>.Ignored)).Returns(new Tenant() { Registers = new() { "test-register" } });

            var response = await RegisterOperations.GetTransaction("test-register", "test-transaction", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTransactionsByDocket_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetTransactionsByDocket("test-register", "test-docket", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.WalletUserRole)]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetTransactionsByDocket_UserInRole_ShouldReturn200Ok(string role)
        {
            A.CallTo(() => _fakeTenantServiceClient!.GetTenantById(A<string>.Ignored)).Returns(new Tenant() { Registers = new() { "test-register" } });

            var response = await RegisterOperations.GetTransactionsByDocket("test-register", "test-docket", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegisters_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetRegisters(false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetRegisters_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await RegisterOperations.GetRegisters(role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegister_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.GetRegister("test-register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.RegisterCreatorRole)]
        [InlineData(Constants.RegisterMaintainerRole)]
        [InlineData(Constants.RegisterReaderRole)]
        public async Task GetRegister_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await RegisterOperations.GetRegister("test-register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateRegister_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.CreateRegister(new {name = "test-register"}, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.RegisterCreatorRole)]
        public async Task CreateRegister_UserInRole_ShouldReturn201Created(string role)
        {
            var response = await RegisterOperations.CreateRegister(new { name = "test-register" }, role: role);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRegister_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await RegisterOperations.DeleteRegister("test-register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.RegisterCreatorRole)]
        public async Task DeleteRegister_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await RegisterOperations.DeleteRegister("test-register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}