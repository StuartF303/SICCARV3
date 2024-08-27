using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Pages.Tenant;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor;
using Xunit;

namespace AdminUiTest.Pages.Client
{
    public class ClientDetailPageTests : TestContext
    {
        private readonly ITenantServiceClient _fakeTenantServiceClient;

        public ClientDetailPageTests()
        {
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_Load_Client_When_ClientId_Specified()
        {
            var tenantId = "test-tenant";
            var clientId = Guid.NewGuid().ToString();
            RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.ClientId!, clientId));
            A.CallTo(() => _fakeTenantServiceClient.Get(tenantId, clientId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Not_Load_Client_When_ClientId_Not_Specified()
        {
            var tenantId = "test-tenant";
            RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.ClientId!, null));
            A.CallTo(() => _fakeTenantServiceClient.Get(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Cancel_Changes_Should_Navigate_To_Tenant_List()
        {
            var tenantId = "test-tenant";
            var component = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.ClientId!, null));

            await component.InvokeAsync(() =>
            {
                var cancelChangesButton = component.FindAll("button").First(b => b.TextContent.Contains("Cancel Changes"));
                cancelChangesButton.Click();
            });

            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            Assert.Equal($"http://localhost/tenants/{tenantId}/", navigationManager.Uri);
        }
    }
}
