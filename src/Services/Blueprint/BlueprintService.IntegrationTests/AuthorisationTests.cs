using System.Net;
using BlueprintService.IntegrationTests.Config;
using FakeItEasy;
using Siccar.Common;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Guid = System.Guid;

namespace BlueprintService.IntegrationTests
{
    public class AuthorisationTests : BlueprintTestBase, IClassFixture<BlueprintWebApplicationFactory<WebHostStartup>>
    {
        public AuthorisationTests(BlueprintWebApplicationFactory<WebHostStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllBlueprints_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.GetAll(false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        [InlineData(Constants.BlueprintAuthoriserRole)]
        public async Task GetAllBlueprints_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await BlueprintOperations!.GetAll(role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetBlueprint_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.Get("test-blueprint", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        [InlineData(Constants.BlueprintAuthoriserRole)]
        public async Task GetBlueprint_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await BlueprintOperations!.Get("test-blueprint", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SaveBlueprint_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.Save(new { }, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        public async Task SaveBlueprint_UserInRole_ShouldReturn202Accepted(string role)
        {
            var response = await BlueprintOperations!.Save(AuthorisationTests.CreateValidBlueprint(), role: role);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        public async Task PublishBlueprint_UserInRole_ShouldReturn202Accepted(string role)
        {
            A.CallTo(() => _fakeTenantServiceClient!.GetTenantById(A<string>._)).Returns(new Tenant { Registers = new() { "register" } });
            var response = await BlueprintOperations!.Publish("address", "register", AuthorisationTests.CreateValidBlueprint(), role: role);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task PublishBlueprint_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.Publish("address", "register", AuthorisationTests.CreateValidBlueprint(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        [InlineData(Constants.BlueprintAuthoriserRole)]
        public async Task GetAllPublishedBlueprints_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await BlueprintOperations!.GetAllPublished("address", "register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllPublishedBlueprints_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.GetAllPublished("address", "register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        public async Task UpdateBlueprint_UserInRole_ShouldReturn202Accepted(string role)
        {
            var response = await BlueprintOperations!.Update(AuthorisationTests.CreateValidBlueprint(), role: role);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBlueprint_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.Update(AuthorisationTests.CreateValidBlueprint(), false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Theory]
        [InlineData(Constants.BlueprintAdminRole)]
        public async Task DeleteBlueprint_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await BlueprintOperations!.Delete("test", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBlueprint_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await BlueprintOperations!.Delete("test", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        private static dynamic CreateValidBlueprint()
        {
            return new
            {
                title = "Blueprint title",
                description = "Blueprint description",
                id = Guid.NewGuid(),
                version = 1,
                dataSchemas = new List<dynamic>
                {
                    new { }
                },
                participants = new List<dynamic>
                {
                    new
                    {
                        id = "test1",
                        name = "test",
                        walletAddress = "test",
                        Organisation= "test"
                    },
                    new
                    {
                        id = "test2",
                        name = "test",
                        walletAddress = "test",
                        Organisation= "test"
                    }
                },
                actions = new List<dynamic>
                {
                    new
                    {
                        id = 1,
                        title = "test",
                        blueprintId = Guid.NewGuid(),
                        dataSchemas = new List<dynamic>
                        {
                            new { }
                        },
                        disclosures = new List<dynamic>
                        {
                            new
                            {
                                dataPointers = new List<string>
                                {
                                    "test"
                                },
                                participantAddress = "test"
                            },
                        },
                        sender = "test",
                        blueprint = Guid.NewGuid(),
                        previousTxId = "00000"
                    }
                }
            };
        }
    }
}