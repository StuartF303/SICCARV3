using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.UI.Admin.Pages.Wallet;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor;
using Xunit;

namespace AdminUiTest.Pages.Wallet
{
    public class WalletDetailTest : TestContext
    {
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IUserServiceClient _fakeUserServiceClient;
        private readonly ITenantServiceClient _fakeTenantServiceClient;

        public WalletDetailTest()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _fakeUserServiceClient = A.Fake<IUserServiceClient>();
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            A.CallTo(() => _fakeUserServiceClient.Get(A<Guid>.Ignored)).Returns(JsonDocument.Parse("{}"));
            A.CallTo(() => _fakeWalletServiceClient.GetWallet(A<string>.Ignored)).Returns(new Siccar.Platform.Wallet
            {
                Owner = Guid.NewGuid().ToString()
            });

            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(_fakeUserServiceClient);
            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
        }

        [Fact]
        public void Should_Load_Wallet_On_Initialization()
        {
            RenderComponent<WalletDetail>(parameters => parameters.Add(p => p.WalletAddress, "test-wallet"));

            A.CallTo(() => _fakeWalletServiceClient.GetWallet("test-wallet")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Load_All_Transactions_On_Initialization()
        {
            RenderComponent<WalletDetail>(parameters => parameters.Add(p => p.WalletAddress, "test-wallet"));

            A.CallTo(() => _fakeWalletServiceClient.GetAllTransactions("test-wallet")).MustHaveHappenedOnceExactly();
        }

        [Fact] 
        public void Should_Load_Wallet_Owner_On_Initialization()
        {
            var userId = Guid.NewGuid();
            A.CallTo(() => _fakeWalletServiceClient.GetWallet("test-wallet")).Returns(new Siccar.Platform.Wallet
            {
                Owner = userId.ToString()
            });

            RenderComponent<WalletDetail>(parameters => parameters.Add(p => p.WalletAddress, "test-wallet"));
            A.CallTo(() => _fakeUserServiceClient.Get(userId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeTenantServiceClient.Get(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }


        [Fact]
        public void OwnerIsClient_ShouldLoadClient_On_Initialization()
        {
            var ownerId = Guid.NewGuid();
            A.CallTo(() => _fakeWalletServiceClient.GetWallet("test-wallet")).Returns(new Siccar.Platform.Wallet
            {
                Owner = ownerId.ToString(),
                Tenant = "test-tenant"
            });

            A.CallTo(() => _fakeUserServiceClient.Get(ownerId))
                .Throws(new HttpStatusException(HttpStatusCode.NotFound, ""));

            RenderComponent<WalletDetail>(parameters => parameters.Add(p => p.WalletAddress, "test-wallet"));
            A.CallTo(() => _fakeUserServiceClient.Get(ownerId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeTenantServiceClient.Get("test-tenant", ownerId.ToString())).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Render_Transactions()
        {
            A.CallTo(() => _fakeWalletServiceClient.GetAllTransactions(A<string>.Ignored)).Returns(new List<WalletTransaction>
            {
                new() { TransactionId = "expected-transaction-a", WalletId = "expected-wallet-id-a", Sender = "expected-sender-a", Timestamp = new DateTime(2023, 02, 01, 09, 12, 24) },
            });

            var cut = RenderComponent<WalletDetail>(parameters =>
            {
                parameters.Add(p => p.WalletAddress, "wallet-address");
            });

            var row = cut.WaitForElement("tr.e-row");

            Assert.NotNull(row);

            var rowCells = row.GetElementsByTagName("td");

            Assert.Equal("expected-transaction-a:expected-wallet-id-a", rowCells[0].InnerHtml);
            Assert.Equal("expected-sender-a", rowCells[1].InnerHtml);
            Assert.Equal("2023-02-01 09:12:24", rowCells[2].InnerHtml);
        }
    }
}
