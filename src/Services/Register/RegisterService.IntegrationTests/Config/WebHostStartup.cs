using Siccar.Registers.RegisterService;

namespace RegisterService.IntegrationTests.Config
{
    public class WebHostStartup : Startup
    {
        public WebHostStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddAuth(IServiceCollection services)
        {
            // Don't add auth
        }
    }
}