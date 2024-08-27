using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Siccar.UI.Admin
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager) : base(provider, navigationManager)
        {
            ConfigureHandler(authorizedUrls: new[] { "https://localhost:8443", "https://localhost:6004" });
            //scopes: new[] { "tenants.manage" });
        }
    }
}
