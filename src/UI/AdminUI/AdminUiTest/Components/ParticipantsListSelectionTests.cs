using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using Syncfusion.Blazor.DropDowns;
using System.Collections.Generic;
using Siccar.Application;
using Xunit;
using System.Threading.Tasks;
using Siccar.UI.Admin.Services;

namespace AdminUiTest.Components
{
    public class ParticipantsListSelectionTests : TestContext
    {
        private readonly ITenantServiceClient _fakeTenantServiceClient;
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;

        public ParticipantsListSelectionTests()
        {
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();

            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_Load_Participants_OnLoad()
        {
            RenderComponent<ParticipantsListSelection>(parameters => parameters.Add(a => a.RegisterId, "test-register"));
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants("test-register")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Select_Participant_Should_Update_Selected_Participants_List()
        {
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants(A<string>.Ignored)).Returns(new List<Participant>
            {
                new()
                {
                    Name = "Test",
                    Organisation = "Test organisation",
                    WalletAddress = "test-wallet-address"
                }
            });

            var component = RenderComponent<ParticipantsListSelection>(parameters => parameters.Add(a => a.RegisterId, "test-register"));

            var participantMultiSelect = component.FindComponent<SfMultiSelect<List<Participant>, Participant>>();
            
            // Open the popup
            await participantMultiSelect.WaitForElement(".e-multi-select-wrapper").MouseDownAsync(new());
            var participantPopup = participantMultiSelect.Find(".e-popup-holder");
            var participantList = participantPopup.QuerySelector("ul");
            var participantListItem = participantList!.QuerySelectorAll("li.e-list-item");
            participantListItem[0].MouseUp(); // Click the first item
            
            Assert.Single(component.Instance.SelectedParticipants);
            Assert.Equal("Test", component.Instance.SelectedParticipants[0].Name);
            Assert.Equal("Test organisation", component.Instance.SelectedParticipants[0].Organisation);
            Assert.Equal("test-wallet-address", component.Instance.SelectedParticipants[0].WalletAddress);
        }

        [Fact]
        public void Should_Load_Participants_WhenRegisterIdUpdated()
        {
            var component = RenderComponent<ParticipantsListSelection>(parameters => parameters.Add(a => a.RegisterId, "test-register"));
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants("test-register")).MustHaveHappenedOnceExactly();

            component.SetParametersAndRender(parameters => parameters.Add(a => a.RegisterId, "new-register"));
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants("new-register")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Not_Load_Participants_WhenRegisterIdNotUpdated()
        {
            var component = RenderComponent<ParticipantsListSelection>(parameters => parameters.Add(a => a.RegisterId, "test-register"));
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants("test-register")).MustHaveHappenedOnceExactly();

            // Set parameters to cause OnParametersSet to fire
            component.SetParametersAndRender(parameters => parameters.Add(a => a.RegisterId, "test-register"));
            component.SetParametersAndRender(parameters => parameters.Add(a => a.SelectedParticipants, new List<Participant>()));
            A.CallTo(() => _fakeTenantServiceClient.GetPublishedParticipants("test-register")).MustHaveHappenedOnceExactly();
        }
    }
}
