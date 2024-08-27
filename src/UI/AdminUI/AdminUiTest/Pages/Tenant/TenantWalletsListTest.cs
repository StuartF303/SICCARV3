using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Pages.Tenant;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using Xunit;

namespace AdminUiTest.Pages.Tenant
{
    public class TenantWalletsListTest : TestContext
    {
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IUserServiceClient _fakeUserServiceClient;

        public TenantWalletsListTest()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _fakeUserServiceClient = A.Fake<IUserServiceClient>();

            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(_fakeUserServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_FetchAllUsersAndWalletsInTenant()
        {
            RenderComponent<TenantWalletsList>();

            A.CallTo(() => _fakeWalletServiceClient.ListWallets(true)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeWalletServiceClient.ListWallets(false)).MustNotHaveHappened();
            A.CallTo(() => _fakeUserServiceClient.All()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_RenderWalletsInTenant()
        {
            var expectedOwnerSub = Guid.NewGuid();

            A.CallTo(() => _fakeWalletServiceClient.ListWallets(true))
                .Returns(new List<Siccar.Platform.Wallet>
                {
                    new() { Name = "Expected wallet name", Address = "expected-address", Owner = expectedOwnerSub.ToString() },
                });

            A.CallTo(() => _fakeUserServiceClient.All())
                .Returns(new List<Siccar.Platform.User>
                {
                    new() { Id = expectedOwnerSub, UserName = "Expected owner name" },
                });

            var cut = RenderComponent<TenantWalletsList>();

            var rows = cut.FindAll(".e-row");

            Assert.Single(rows);

            Assert.Equal("Expected wallet name", rows[0].Children[0].InnerHtml);
            Assert.Equal("expected-address", rows[0].Children[1].InnerHtml);
            Assert.Equal("Expected owner name", rows[0].Children[2].InnerHtml);
        }
    }
}
