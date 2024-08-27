using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Siccar.UI.Admin.Pages.Tenant;
using Xunit;
using FakeItEasy;
using Siccar.Common.ServiceClients;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using Bunit.TestDoubles;
using Siccar.UI.Admin.Shared.Components;
using Siccar.UI.Admin.Services;

namespace AdminUiTest.Pages.Client
{
    public class ClientListTests : TestContext
    {
        private readonly ITenantServiceClient _fakeTenantServiceClient;
        private readonly FakeNavigationManager _navMan;
        private readonly List<Siccar.Common.ServiceClients.Models.Tenant.Client> _clients;

        public ClientListTests()
        {
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            _clients = new List<Siccar.Common.ServiceClients.Models.Tenant.Client>
            {
                new()
                {
                    ClientId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    ClientName = "Test Client",
                    Description = "A Test Client"
                }
            };

            A.CallTo(() => _fakeTenantServiceClient.ListClients(A<string>.Ignored)).Returns(_clients);

            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            _navMan = Services.GetRequiredService<FakeNavigationManager>();
        }

        [Fact]
        public void Should_Load_Client_List()
        {
            RenderComponent<ClientList>(parameters => parameters.Add(p => p.TenantId, "test-tenant-id"));
            A.CallTo(() => _fakeTenantServiceClient.ListClients("test-tenant-id")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Add_Client_Should_Navigate_To_Client_Detail_Page()
        {
            var tenantId = "test-tenant-id";
            var clientList = RenderComponent<ClientList>(parameters => parameters.Add(p => p.TenantId, tenantId));
            var buttonElement = clientList.Find("button");
            buttonElement.Click();
            Assert.Equal($"http://localhost/tenants/{tenantId}/clients/new", _navMan.Uri);
        }

        [Fact]
        public void Click_Client_Should_Navigate_To_Client_Detail_Page()
        {
            var tenantId = "test-tenant-id";
            var clientList = RenderComponent<ClientList>(parameters => parameters.Add(p => p.TenantId, tenantId));
            var clientRow = clientList.FindAll("td").FirstOrDefault(e => e.TextContent.Contains("A Test Client"));
            Assert.NotNull(clientRow);
            clientRow!.Click();
            Assert.Equal($"http://localhost/tenants/{tenantId}/clients/{_clients.First().ClientId}", _navMan.Uri);
        }

        [Fact]
        public void Click_Delete_Should_Show_Delete_Dialog()
        {
            var tenantId = "test-tenant-id";
            var clientList = RenderComponent<ClientList>(parameters => parameters.Add(p => p.TenantId, tenantId));
            var deleteButton = clientList.FindAll("button").FirstOrDefault(e => e.TextContent.Contains('X'));
            var dialog = clientList.FindComponent<Dialog>();
            Assert.NotNull(deleteButton);
            Assert.NotNull(dialog);
            Assert.False(dialog.Instance.IsVisible);
            deleteButton!.Click();
            Assert.True(dialog.Instance.IsVisible);
        }

        [Fact] 
        public void Confirm_Delete_Should_Delete_Client()
        {
            var tenantId = "test-tenant-id";
            var clientList = RenderComponent<ClientList>(parameters => parameters.Add(p => p.TenantId, tenantId));
            var deleteButton = clientList.FindAll("button").FirstOrDefault(e => e.TextContent.Contains('X'));
            var dialog = clientList.FindComponent<Dialog>();
            Assert.NotNull(deleteButton);
            Assert.NotNull(dialog);
            deleteButton!.Click();

            var okButton = dialog.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("OK"));
            okButton!.Click();
            A.CallTo(() => _fakeTenantServiceClient.DeleteClient(tenantId, _clients.First().ClientId!)).MustHaveHappenedOnceExactly();
        }
    }
}
