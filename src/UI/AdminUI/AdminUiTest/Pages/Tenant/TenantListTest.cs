using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Platform.Tenants.Core;
using Siccar.UI.Admin.Models;
using Siccar.UI.Admin.Pages.Tenant;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Grids;
using System.Linq;
using Bunit.TestDoubles;
using FakeItEasy;
using SiccarClient = Siccar.Platform.Tenants.Core.Client;
using Syncfusion.Blazor.Charts.Chart.Internal;
using Siccar.UI.Admin.Services;

namespace AdminUiTest.Pages.Tenant
{
    public class TenantListTest : TestContext
    {
        private List<Siccar.Platform.Tenant> tenantsList = new List<Siccar.Platform.Tenant>();
        private readonly ITenantServiceClient fakeTenantService;

        public TenantListTest()
        {
            fakeTenantService = A.Fake<ITenantServiceClient>();
            tenantsList!.AddRange(new List<Siccar.Platform.Tenant> {
                new Siccar.Platform.Tenant
                {
                    Id = "78b4786f-76e7-4daf-a6d8-3f629ec1397c",
                    Name = "Alpha",
                    Authority = "Authority22",
                    Clients = new List<SiccarClient>() { new SiccarClient { Id = Guid.NewGuid().ToString() } },
                    Registers = new List<string>() { "string1", "string2", "string3", "string4" },
                    AccountsCount = 1,
                },
                new Siccar.Platform.Tenant
                {
                    Id = "c5d017c0-4f21-4a37-896f-a4b3ff099534",
                    Name = "Bravo",
                    Authority = "Authority22",
                    Clients = new List<SiccarClient>() { new SiccarClient { Id = Guid.NewGuid().ToString() } },
                    Registers = new List<string>() { "string1", "string2", "string3", "string4" },
                    AccountsCount = 2,
                },
                new Siccar.Platform.Tenant
                {
                    Id = "117d7dce-0d0f-49c0-a0c5-a90b2a0c06c1",
                    Name = "Charlie",
                    Authority = "Authority22",
                    Clients = new List<SiccarClient>() { new SiccarClient { Id = Guid.NewGuid().ToString() } },
                    Registers = new List<string>() { "string1", "string2", "string3", "string4" },
                    AccountsCount = 4,
                }});

            A.CallTo(() => fakeTenantService.All()).Returns(Task.FromResult(tenantsList));

            Services.AddSingleton<ITenantServiceClient>(fakeTenantService);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            JSInterop.Setup<Browser>("sfBlazor.Chart.getBrowserDeviceInfo");
        }

        [Fact]
        public void TenantListInitialSortIsSetToFirstColumnAscending()
        {
            // Arrange
            using var cut = RenderComponent<TenantList>();

            // Act
            var initialSort = cut.Instance.Grid.SortSettings.Columns;

            // Assert
            Assert.Equal("Name", initialSort[0].Field);
            Assert.Equal("Ascending", initialSort[0].Direction.ToString());
        }

        [Fact]
        public async Task TenantListClickSelectsClientsColumnAscending()
        {
            // Arrange
            using var cut = RenderComponent<TenantList>();

            // Act
            await cut.InvokeAsync(() =>
            {
                cut.FindAll(".e-headercell").ElementAt(3).Click(detail: 5);
                var subsequentSort = cut.Instance.Grid.SortSettings.Columns;
                // Assert
                Assert.Equal("Clients", subsequentSort[0].Field);
                Assert.Equal("Ascending", subsequentSort[0].Direction.ToString());
            });

        }

        [Fact]
        public async void TenantListNavigatesToCorrectUriWhenGridFirstRowIsClicked()
        {
            // Arrange
            using var cut = RenderComponent<TenantList>();

            // Act
            var NavManager = Services.GetRequiredService<FakeNavigationManager>();
            await cut.InvokeAsync(async () => await cut.Instance.Grid.SelectRowAsync(0));

            // Assert
            Assert.Equal($"http://localhost/tenants/{tenantsList[0].Id}/", NavManager.Uri);
            A.CallTo(() => fakeTenantService.All()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void TenantListNavigatesToCorrectUriWhenGridSecondRowIsClicked()
        {
            // Arrange
            using var cut = RenderComponent<TenantList>();

            // Act
            var NavManager = Services.GetRequiredService<FakeNavigationManager>();
            await cut.InvokeAsync(async () => await cut.Instance.Grid.SelectRowAsync(1));

            // Assert
            Assert.Equal($"http://localhost/tenants/{tenantsList[1].Id}/", NavManager.Uri);
        }


        [Fact]
        public void ListTenantComponentRendersCorrectly()
        {
            // Arrange
            var cut = RenderComponent<TenantList>();

            // Act
            var sfGrid = cut.FindComponent<SfGrid<TenantModel>>();

            // Assert
            Assert.Equal(3, sfGrid.FindAll(".e-row").Count);
            Assert.Equal(5, sfGrid.Find(".e-row").Children.Count());
            Assert.Equal(3, cut.Instance.Tenants.Count);
            A.CallTo(() => fakeTenantService.All()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ListTenantComponentRendersRowCorrectly()
        {
            // Arrange
            var cut = RenderComponent<TenantList>();

            // Act
            var sfGrid = cut.FindComponent<SfGrid<TenantModel>>();

            // Assert
            var rows = sfGrid.FindAll(".e-row");
            Assert.Equal(3, rows.Count);

            Assert.Equal(tenantsList[0].Name, rows[0].Children[0].InnerHtml);
            Assert.Equal(tenantsList[0].Authority, rows[0].Children[1].InnerHtml);
            Assert.Equal(tenantsList[0].AccountsCount.ToString(), rows[0].Children[2].InnerHtml);
            Assert.Equal(tenantsList[0].Clients.Count().ToString(), rows[0].Children[3].InnerHtml);
            Assert.Equal(tenantsList[0].Registers.Count().ToString(), rows[0].Children[4].InnerHtml);
        }
    }
}
