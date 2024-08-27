using System.Net;
using ActionService.IntegrationTests.Config;

namespace ActionService.IntegrationTests
{
    public class AuthorisationTests : ActionTestBase, IClassFixture<ActionWebApplicationFactory<WebHostStartup>>
    {
        public AuthorisationTests(ActionWebApplicationFactory<WebHostStartup> factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetAllStartingActions_UserInRole_ShouldReturn200Ok()
        {
            var response = await ActionOperations.GetStartingActions("test", "test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllStartingActions_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await ActionOperations.GetStartingActions("test", "test", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllActions_UserInRole_ShouldReturn200Ok()
        {
            var response = await ActionOperations.GetAll("test", "test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllActions_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await ActionOperations.GetAll("test", "test", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetById_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await ActionOperations.GetById("test", "test", "test", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Submit_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await ActionOperations.Submit(null, false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}