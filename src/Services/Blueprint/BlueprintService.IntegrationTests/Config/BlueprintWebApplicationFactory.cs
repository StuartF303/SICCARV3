using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlueprintService.IntegrationTests.Config
{
    public class BlueprintWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private static IWebHostBuilder? _webHostBuilder;
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            if (_webHostBuilder == null)
            {
                _webHostBuilder = WebHost.CreateDefaultBuilder(Array.Empty<string>())
                    .UseStartup<TEntryPoint>();
                return _webHostBuilder;
            }

            return _webHostBuilder;
        }
    }
}
