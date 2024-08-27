using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Bunit;
using Siccar.UI.Admin.Pages.Tenant;
using Siccar.Common.ServiceClients;
using Syncfusion.Blazor;
using Microsoft.Extensions.DependencyInjection;
using FakeItEasy;
using Microsoft.AspNetCore.Components.Authorization;
using Siccar.UI.Admin.Shared;
using Syncfusion.Blazor.Inputs;
using Siccar.UI.Admin.Services;

namespace AdminUiTest.Pages.Tenant
{
    public class TenantInfoTest : TestContext
    {
        private readonly Siccar.Platform.Tenant _tenant;
        private readonly ITenantServiceClient _fakeTenantServiceClient;
        public TenantInfoTest()
        {
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            var fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            var fakeAuthenticationStateProvider = A.Fake<AuthenticationStateProvider>();

            _tenant = new Siccar.Platform.Tenant
            {
                Id = Guid.NewGuid().ToString(),
                AdminEmail = "dev@siccar.net",
                BillingEmail = "dev@siccar.net",
                Name = "Siccar",
                Registers = new List<string>()
            };
            A.CallTo(() => _fakeTenantServiceClient.GetTenantById(A<string>._)).Returns(_tenant);

            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(fakeRegisterServiceClient);
            Services.AddSingleton(fakeAuthenticationStateProvider);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_RenderEditableCards()
        {

            // Act
            var result = RenderComponent<TenantInfo>();

            // Assert
            var cards = result.FindComponents<EditableCard>();

            Assert.Equal(3, cards.Count);
        }

        [Fact]
        public void Should_RenderAdminEmailCard()
        {
            // Act
            var result = RenderComponent<TenantInfo>();

            // Assert
            var cards = result.FindComponents<EditableCard>().ToList();
            var card = cards.Find(card => card.Instance.ID == "Admin");
            Assert.Equal(_tenant.AdminEmail, card?.Instance.Value);
        }

        [Fact]
        public void Should_RenderOrgCard()
        {
            // Act
            var result = RenderComponent<TenantInfo>();

            // Assert
            var cards = result.FindComponents<EditableCard>().ToList();
            var card = cards.Find(card => card.Instance.ID == "Org");
            Assert.Equal(_tenant.Name, card?.Instance.Value);
        }

        [Fact]
        public void Should_RenderBillingEmailCard()
        {
            // Act
            var result = RenderComponent<TenantInfo>();

            // Assert
            var cards = result.FindComponents<EditableCard>().ToList();
            var card = cards.Find(card => card.Instance.ID == "Billing");
            Assert.Equal(_tenant.BillingEmail, card?.Instance.Value);
        }

        [Fact]
        public async Task Should_UpdateTenantOnOutFocusAsync()
        {
            var tenantId = Guid.NewGuid().ToString();
            var updatedValue = "differentOrgValue";
            // Act
            var result = RenderComponent<TenantInfo>(parameters => parameters.Add(param => param.SelectedTenantId, tenantId));

            // Assert
            var cards = result.FindComponents<EditableCard>().ToList();
            var card = cards.Find(card => card.Instance.ID == "Org");
            var textbox = card?.FindComponent<SfTextBox>().Find("input");
            Assert.NotNull(textbox);
            if (textbox is not null)
            {
                await textbox.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());
                await textbox.ChangeAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = updatedValue });
                await textbox.FocusOutAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());
                A.CallTo(() => _fakeTenantServiceClient.UpdateTenant(tenantId,
                    A<Siccar.Platform.Tenant>.That.Matches(org => org.Name == updatedValue))).MustHaveHappenedOnceExactly();
            }
        }
    }
}
