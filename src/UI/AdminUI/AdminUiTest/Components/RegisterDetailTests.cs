using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.UI.Admin.Adaptors;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Charts.Chart.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdminUiTest.Components
{
    public class RegisterDetailTests : TestContext
    {
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly TransactionAdaptorOData _transactionAdaptorOData;
        private readonly string _testRegisterId = "test-register-id";

        public RegisterDetailTests()
        {
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _transactionAdaptorOData = new TransactionAdaptorOData(_fakeRegisterServiceClient);
            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSingleton(_transactionAdaptorOData);
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            JSInterop.Setup<Browser>("sfBlazor.Chart.getBrowserDeviceInfo");
        }

        [Fact]
        public void Should_Load_Register_On_Load()
        {
            var _ = RenderComponent<RegisterDetail>(parameters => parameters.Add(p => p.RegisterId, _testRegisterId));
            A.CallTo(() => _fakeRegisterServiceClient.GetRegister(_testRegisterId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Render_Row_Correctly()
        {
            A.CallTo(() => _fakeRegisterServiceClient.GetAllTransactionsOData(A<string>.Ignored, A<string>.Ignored)).Returns(new ODataRaw<List<TransactionModel>>()
            {
                Value = new List<TransactionModel>()
                {
                    new() {
                        TxId = "expected-tx-id",
                        SenderWallet = "expected-sender-wallet",
                        TimeStamp = new(2023, 02, 03, 13, 48, 10),
                    }
                },
            });

            var cut = RenderComponent<RegisterDetail>(parameters =>
            {
                parameters.Add(p => p.RegisterId, "some-reg");
            });

            var row = cut?.WaitForElements("tr.e-row")?.LastOrDefault();
            Assert.NotNull(row);

            var cells = row.GetElementsByTagName("td");

            Assert.Equal("expected-tx-id", cells[0].InnerHtml);
            Assert.Equal("expected-sender-wallet", cells[1].InnerHtml);
            Assert.Equal("2023-02-03 13:48:10", cells[2].InnerHtml);

        }
    }
}
