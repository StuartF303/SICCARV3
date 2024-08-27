using System.Net;
using PeerService.IntegrationTests.Config;
using Siccar.Common;

namespace PeerService.IntegrationTests
{
    public class AuthorisationTests : PeerTestBase, IClassFixture<PeerWebApplicationFactory<WebHostStartup>>
    {
        public AuthorisationTests(PeerWebApplicationFactory<WebHostStartup> factory) : base(factory)
        {

        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        public async Task GetPeer_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await PeerOperations.GetPeer(role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        [InlineData(Constants.InstallationReaderRole)]
        public async Task GetPeers_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await PeerOperations.GetPeers(role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        public async Task PostPeer_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await PeerOperations.PostPeer(new {id = "test"}, role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(Constants.InstallationAdminRole)]
        public async Task PostHostRegister_UserInRole_ShouldReturn200Ok(string role)
        {
            var response = await PeerOperations.PostHostRegister("test-register", role: role);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostHostRegister_UserNotInRole_ShouldReturn403Forbidden()
        {
            var response = await PeerOperations.PostHostRegister("test-register", false);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}