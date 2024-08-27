using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using static Google.Rpc.Context.AttributeContext.Types;

namespace WalletService.IntegrationTests.Config
{
    public class WebHostStartup : Startup
    {
        private readonly bool _bypassAuthorisation;

        public WebHostStartup(IConfiguration configuration, bool bypassAuthorisation) : base(configuration)
        {
            _bypassAuthorisation = bypassAuthorisation;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(o => o.SuppressCheckForUnhandledSecurityMetadata = true);
            base.ConfigureServices(services);
        }

        public override void AddAuth(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>()
            {
                { "role", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" }
            };
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (_bypassAuthorisation)
            {
                app.UseMiddleware<AutoAuthoriseMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
