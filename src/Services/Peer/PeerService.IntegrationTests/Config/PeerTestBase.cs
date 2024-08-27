using System.Reflection;
using Microsoft.AspNetCore.TestHost;
using WebMotions.Fake.Authentication.JwtBearer;

namespace PeerService.IntegrationTests.Config
{
    [Collection("Peer")]
    public class PeerTestBase
    {
        public readonly PeerOperations PeerOperations;
        private readonly PeerWebApplicationFactory<WebHostStartup> _factory;

        public PeerTestBase(PeerWebApplicationFactory<WebHostStartup> factory)
        {
            _factory = factory;
            PeerOperations = new(CreateClient());
        }

        private HttpClient CreateClient()
        {
            var path = Assembly.GetAssembly(typeof(WebHostStartup))!.Location;

            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Fake auth
                        services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                    });

                    builder.UseStartup<WebHostStartup>();
                    builder.UseContentRoot(Path.GetDirectoryName(path)!);
                    builder.ConfigureAppConfiguration(cb =>
                    {
                        cb.AddJsonFile("appsettings.Development.json", optional: false)
                            .AddEnvironmentVariables();
                    });
                })
                .CreateClient();

            return client;
        }
    }
}
