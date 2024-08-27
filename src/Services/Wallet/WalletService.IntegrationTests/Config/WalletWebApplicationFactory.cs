using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WalletService.IntegrationTests.Config
{
    public class WalletWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TEntryPoint>();
        }
    }
}
