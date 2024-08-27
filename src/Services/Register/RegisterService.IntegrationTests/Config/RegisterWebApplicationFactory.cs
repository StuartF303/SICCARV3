using Castle.Core.Internal;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RegisterService.IntegrationTests.Config
{
    public class RegisterWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TEntryPoint>();
        }
    }
}