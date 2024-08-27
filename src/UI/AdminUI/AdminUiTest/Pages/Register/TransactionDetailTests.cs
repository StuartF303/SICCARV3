using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.UI.Admin.Pages.Register;
using Siccar.UI.Admin.Services;
using Syncfusion.Blazor;
using Syncfusion.ExcelExport;
using System;
using Xunit;

namespace AdminUiTest.Pages.Register
{
    public class TransactionDetailTests : TestContext
    {
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;

        public TransactionDetailTests()
        {
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();

            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_Load_Transaction_On_Load()
        {
            RenderComponent<TransactionDetail>(parameters =>
            {
                parameters.Add(p => p.RegisterId, "expected-register-id");
                parameters.Add(p => p.selectedTransactionTxId, "expected-tx-id");
            });

            A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById("expected-register-id", "expected-tx-id")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Show_Submitted_And_Validated_Date()
        {
            A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(new TransactionModel
            {
                TimeStamp = new DateTime(2023, 02, 03, 14, 22, 05),
            });

            var cut = RenderComponent<TransactionDetail>();

            var submittedCell = cut.WaitForElement("[data-testid=submitted-datetime]");
            var validatedCell = cut.WaitForElement("[data-testid=validated-datetime]");

            Assert.Equal("2023-02-03 14:22:05", submittedCell.Children[0].InnerHtml);
            Assert.Equal("2023-02-03 14:22:05", validatedCell.Children[0].InnerHtml);
        }
    }
}
