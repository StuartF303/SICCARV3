using Bunit;
using Bunit.TestDoubles;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Pages.Blueprint;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siccar.UI.Admin.Services;
using Xunit;
using Blazored.LocalStorage;

namespace AdminUiTest.Pages.Blueprint
{
    public class BlueprintListTest : TestContext
    {
        private readonly FakeNavigationManager _navMan;
        private readonly IBlueprintServiceClient _fakeBlueprintServiceClient;
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly List<Siccar.Platform.Register> _expectedRegisters;
        private readonly ILocalStorageService _localStorageService;

        public BlueprintListTest()
        {
            _fakeBlueprintServiceClient = A.Fake<IBlueprintServiceClient>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();

            _expectedRegisters = new List<Siccar.Platform.Register>
            {
                new() { Id = Guid.NewGuid().ToString() },
                new() { Id = Guid.NewGuid().ToString() }
            };

            A.CallTo(() => _fakeRegisterServiceClient.GetRegisters()).Returns(_expectedRegisters);

            Services.AddSingleton(_fakeBlueprintServiceClient);
            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            _localStorageService = this.AddBlazoredLocalStorage();
            _navMan = Services.GetRequiredService<FakeNavigationManager>();
            //JSInterop.Setup<String>("localStorage.getItem", _ => true);
        }

        [Fact]
        public async Task Should_NavigateToBlueprintCreatePage_OnClick()
        {
            var component = RenderComponent<BlueprintList>();

            await component.InvokeAsync(() =>
            {
                var buttons = component.FindComponent<SfButton>().FindAll("button");
                var saveButton = buttons.First(button => button.TextContent.ToLowerInvariant().Contains("create blueprint"));
                saveButton.Click();

            });
            Assert.Equal($"http://localhost/blueprints/create", _navMan.Uri);
        }

        [Fact]
        public void Should_List_Blueprints_And_Wallets_OnLoad()
        {
            RenderComponent<BlueprintList>();

            A.CallTo(() => _fakeRegisterServiceClient.GetRegisters()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_Load_Published_Blueprints_On_Selecting_Register_Selected()
        {
            var component = RenderComponent<BlueprintList>();

            // Show the register dropdown list popup
            await component.InvokeAsync(async () =>
            {
                var registerDropdownList = component.FindComponent<SfDropDownList<string, Siccar.Platform.Register>>();
                await registerDropdownList.Instance.ShowPopup();
                var registerPopup = registerDropdownList.Find(".e-popup");
                var registerList = registerPopup.QuerySelector("ul");
                var registerListCollection = registerList!.QuerySelectorAll("li.e-list-item");
                registerListCollection[0].Click(); // Click the first item
            });

            A.CallTo(() => _fakeBlueprintServiceClient.GetAllPublished(
            A<string>.That.Matches(register => register == _expectedRegisters[0].Id))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_GetBlueprintsForStoredRegisterId()
        {
            var expectedRegisterId = _expectedRegisters[1].Id;
            await _localStorageService.SetItemAsync("register_id", expectedRegisterId);
            A.CallTo(() => _fakeBlueprintServiceClient.GetAllPublished(A<string>._))
                .Returns((new List<Siccar.Application.Blueprint>{ new Siccar.Application.Blueprint()}));

            var component = RenderComponent<BlueprintList>();

            component.WaitForState(() => component.Instance.grid.DataSource.Any());

            A.CallTo(() => _fakeBlueprintServiceClient.GetAllPublished(
                A<string>.That.Matches(register => register == _expectedRegisters[1].Id))).MustHaveHappened();
        }

        [Fact]
        public async Task Should_Not_Load_Published_Blueprints_On_Selecting_Register_And_No_Wallet_Selected()
        {
            var component = RenderComponent<BlueprintList>();

            // Show the register dropdown list popup
            var registerDropdownList = component.FindComponent<SfDropDownList<string, Siccar.Platform.Register>>();
            await registerDropdownList.Instance.ShowPopup();
            var registerPopup = registerDropdownList.Find(".e-popup");
            var registerList = registerPopup.QuerySelector("ul");
            var registerListCollection = registerList!.QuerySelectorAll("li.e-list-item");
            registerListCollection[0].Click(); // Click the first item


            A.CallTo(() => _fakeBlueprintServiceClient.GetAllPublished(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}
