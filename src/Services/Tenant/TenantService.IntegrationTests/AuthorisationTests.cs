using System.Linq.Expressions;
using System.Net;
using FakeItEasy;
using Siccar.Common;
using Siccar.Platform;
using TenantService.IntegrationTests.Config;

namespace TenantService.IntegrationTests
{
    public class AuthorisationTests : TenantTestBase, IClassFixture<TenantWebApplicationFactory<WebHostStartup>>
    {
        public AuthorisationTests(TenantWebApplicationFactory<WebHostStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task ListTenants_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.ListTenants(false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        [InlineData(Constants.TenantAdminRole)]
        [InlineData(Constants.TenantBillingRole)]
        public async Task ListTenants_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.ListTenants(role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClients_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.GetAllClients("test-tenant", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        [InlineData(Constants.TenantAdminRole)]
        [InlineData(Constants.TenantBillingRole)]
        public async Task GetAllClients_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.GetAllClients("test-tenant", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetClient_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.GetClient("test-tenant", "test-client", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        [InlineData(Constants.TenantAdminRole)]
        [InlineData(Constants.TenantBillingRole)]
        public async Task GetClient_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.GetClient("test-tenant", "test-client", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClient_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.UpdateClient("test-tenant", new {clientId = "test-client"}, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        [InlineData(Constants.TenantAdminRole)]
        public async Task UpdateClient_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.UpdateClient("test-tenant", new { clientId = "test-client", tenantId="test-tenant", clientName="test-client" }, role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTenant_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.GetTenant("test-tenant", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        [InlineData(Constants.TenantAdminRole)]
        [InlineData(Constants.TenantBillingRole)]
        public async Task GetTenant_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.GetTenant("test-tenant", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateTenant_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.CreateTenant(new {id = Guid.NewGuid().ToString()}, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        public async Task CreateTenant_UserInRole_ShouldReturn202Accepted(string role)
        {
            var response = await TenantOperations.CreateTenant(new
            {
                id = Guid.NewGuid().ToString(),
                name = "test",
                adminEmail = "test@siccar.net",
                billingEmail = "test@siccar.net",
            },
            role: role);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenant_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.UpdateTenant(new { id = Guid.NewGuid().ToString() }, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        public async Task UpdateTenant_UserInRole_ShouldReturn202Accepted(string role)
        {
            var response = await TenantOperations.UpdateTenant(new
                {
                    id = Guid.NewGuid().ToString(),
                    name = "test",
                    adminEmail = "test@siccar.net",
                    billingEmail = "test@siccar.net",
                },
                role: role);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTenant_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.DeleteTenant("test-tenant", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.TenantAdminRole)]
        public async Task DeleteTenant_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.DeleteTenant("test-tenant", role:role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateClient_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.CreateClient("test-tenant", new {}, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.TenantAdminRole)]
        public async Task CreateClient_UserInRole_ShouldReturn201Created(string role)
        {
            var response = await TenantOperations.CreateClient("test-tenant", new { tenantId="test-tenant", clientId = "test"}, role: role);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClient_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await TenantOperations.DeleteClient("test-tenant", "test-client", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.TenantAdminRole)]
        public async Task DeleteClient_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await TenantOperations.DeleteClient("test-tenant", "test-client", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PublishParticipant_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response =
                await TenantOperations.PublishParticipant("test-register", "test-address", new { },
                    false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.TenantAdminRole)]
        [InlineData(Constants.BlueprintAdminRole)]
        public async Task PublishParticipant_UserInRole_ShouldReturn201Created(string role)
        {
            A.CallTo(() => _fakeTenantRepository!.Single<Tenant>(A<Expression<System.Func<Tenant, bool>>>.Ignored))
                .Returns(new Tenant() { Id = "test", Registers = new List<string>() { "test-register" } });
            var response =
                await TenantOperations.PublishParticipant("test-register", "test-address", 
                    new
                    {
                        id= "test", 
                        name = "test", 
                        walletAddress = "test-address",
                        organisation = "org"
                    },
                    role: role);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}