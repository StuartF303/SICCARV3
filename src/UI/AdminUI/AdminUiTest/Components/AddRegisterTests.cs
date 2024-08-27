using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Syncfusion.Blazor;
using System.Linq;
using Siccar.Platform;
using Siccar.UI.Admin.Shared.Components;
using Xunit;
using Bunit.TestDoubles;
using Syncfusion.Blazor.Charts.Chart.Internal;

namespace AdminUiTest.Components
{
    public class AddRegisterTests : TestContext
    {
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;

        public AddRegisterTests()
        {
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            JSInterop.Setup<Browser>("sfBlazor.Chart.getBrowserDeviceInfo");
        }

        [Fact]
        public void Valid_Register_Should_Create_Register_On_Button_Click()
        {
            var addRegisterComponent = RenderComponent<AddRegister>();
            addRegisterComponent.Find("input").Change("Test-Register");

            var saveButton = addRegisterComponent.FindAll("button").First(b => b.InnerHtml.Contains("Add Register"));
            saveButton.Click();

            A.CallTo(() => _fakeRegisterServiceClient.CreateRegister(A<Register>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Invalid_Register_Should_Not_Create_Register_On_Button_Click()
        {
            var addRegisterComponent = RenderComponent<AddRegister>();
            addRegisterComponent.Find("input").Change("");

            var saveButton = addRegisterComponent.FindAll("button").First(b => b.InnerHtml.Contains("Add Register"));
            saveButton.Click();

            A.CallTo(() => _fakeRegisterServiceClient.CreateRegister(A<Register>._)).MustNotHaveHappened();
        }
    }
}
