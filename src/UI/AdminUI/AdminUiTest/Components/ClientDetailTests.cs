using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using FakeItEasy;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Common.ServiceClients.Models.Tenant;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Charts.Chart.Internal;
using Xunit;
using ClientDetail = Siccar.UI.Admin.Shared.Components.ClientDetail;

namespace AdminUiTest.Components
{
    public class ClientDetailTests : TestContext
    {
        private readonly ITenantServiceClient _fakeTenantServiceClient;

        public ClientDetailTests()
        {
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSyncfusionBlazor();
            Services.AddOptions();

            JSInterop.Setup<Browser>("sfBlazor.Chart.getBrowserDeviceInfo");
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [Fact]
        public async Task New_Client_Should_Create_Client_When_Saved()
        {
            JSInterop.SetupVoid("ChangeUrl", "/tenants/test-tenant/clients/");
            var tenantId = "test-tenant";
            var clientDetailComponent = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.Client, null));

            // Make the form valid
            clientDetailComponent.Instance.Client!.ClientName = "test";
            // Click the save button
            await clientDetailComponent.InvokeAsync(() =>
            {
                var saveButton = clientDetailComponent.FindAll("button").First(b => b.InnerHtml.Contains("Save changes"));
                saveButton.Click();
            });
            A.CallTo(() => _fakeTenantServiceClient.Create(A<Client>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Existing_Client_Should_Update_Client_When_Saved()
        {
            var tenantId = "test-tenant";
            var clientId = Guid.NewGuid();

            var client = new Client
            {
                Id = clientId,
                ClientId = clientId.ToString(),
                TenantId = tenantId,
                ClientName = "Test Client"
            };

            var clientDetailComponent = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.Client, client));

            await clientDetailComponent.InvokeAsync(() =>
            {
                var saveButton = clientDetailComponent.FindAll("button").First(b => b.InnerHtml.Contains("Save changes"));
                saveButton.Click();
            });
            A.CallTo(() => _fakeTenantServiceClient.ClientUpdate(A<Client>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Invalid_Form_Should_Not_Save_Client_When_Saved()
        {
            var tenantId = "test-tenant";
            var clientId = Guid.NewGuid();

            var client = new Client
            {
                Id = clientId,
                ClientId = clientId.ToString(),
                TenantId = tenantId,
                ClientName = "Test Client"
            };

            var clientDetailComponent = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.Client, client));

            client.ClientName = "";
            var saveButton = clientDetailComponent.FindAll("button").First(b => b.InnerHtml.Contains("Save changes"));
            saveButton.Click();
            A.CallTo(() => _fakeTenantServiceClient.ClientUpdate(A<Client>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeTenantServiceClient.Create(A<Client>._)).MustNotHaveHappened();
        }

        [Fact]
        public void No_ClientSecrets_When_Client_Secret_Required_Checked_Should_Add_New_ClientSecret()
        {
            var tenantId = "test-tenant";
            var clientId = Guid.NewGuid();

            var client = new Client
            {
                Id = clientId,
                ClientId = clientId.ToString(),
                TenantId = tenantId,
                ClientName = "Test Client"
            };

            var clientDetail = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.Client, client));
            Assert.Empty(client.ClientSecrets);

            var editForm = clientDetail.FindComponent<EditForm>();
            var checkbox = editForm.FindAll("input").First(c => c.Id == "require-client-secret");
            checkbox.Change(true);
            Assert.NotEmpty(client.ClientSecrets);
        }

        [Fact]
        public async Task Cancel_Changes_Should_Navigate_To_Tenant_List()
        {
            var tenantId = "test-tenant";
            var component = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId));

            await component.InvokeAsync(() =>
            {
                var cancelChangesButton = component.FindAll("button").First(b => b.TextContent.Contains("Cancel Changes"));
                cancelChangesButton.Click();
            });

            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            Assert.Equal($"http://localhost/tenants/{tenantId}/", navigationManager.Uri);
        }

        [Fact]
        public void Click_Copy_Client_Secret_Should_Copy_To_Keyboard()
        {
            var client = new Client
            {
                RequireClientSecret = true
            };

            var tenantId = "test-tenant";

            var component = RenderComponent<ClientDetail>(parameters => parameters.Add(p => p.TenantId, tenantId).Add(p => p.Client, client));

            var editForm = component.FindComponent<EditForm>();
            var checkbox = editForm.FindAll("input").First(c => c.Id == "require-client-secret");
            checkbox.Change(true);

            // Click the Copy button
            var copySharedSecretButton = component.FindAll("button").First(b => b.TextContent.Contains("Copy"));
            copySharedSecretButton.Click();

            Assert.Contains(JSInterop.Invocations, i => i.Identifier == "navigator.clipboard.writeText" && Guid.TryParse(i.Arguments[0]!.ToString(), out _));
        }

    }
}
