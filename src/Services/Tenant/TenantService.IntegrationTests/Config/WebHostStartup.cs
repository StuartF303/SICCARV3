using IdentityServer4;
using Siccar.Platform.Tenants;

namespace TenantService.IntegrationTests.Config
{
    public class WebHostStartup : Startup
    {
        public WebHostStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddAuth(IServiceCollection services)
        {

        }
    }
}
