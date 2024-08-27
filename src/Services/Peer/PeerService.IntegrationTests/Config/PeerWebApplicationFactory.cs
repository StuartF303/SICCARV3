using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PeerService.IntegrationTests.Config
{
    public class PeerWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TEntryPoint>();
        }
    }
}
