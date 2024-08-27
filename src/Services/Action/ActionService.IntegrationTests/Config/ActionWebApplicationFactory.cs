using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ActionService.IntegrationTests.Config
{
    public class ActionWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TEntryPoint>();
        }
    }
}
