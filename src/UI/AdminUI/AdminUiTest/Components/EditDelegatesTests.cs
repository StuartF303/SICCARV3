using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Syncfusion.Blazor.Charts.Chart.Internal;
using Syncfusion.Blazor;
using Xunit;
using Siccar.UI.Admin.Shared.Components;
using AngleSharp.Dom;
using System.Linq;
using Siccar.Platform;
using System.Collections.Generic;
using System;
using Syncfusion.Blazor.DropDowns;
using Siccar.UI.Admin.Models;
using System.Threading.Tasks;

namespace AdminUiTest.Components
{
    public class EditDelegatesTests : TestContext
    {
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IUserServiceClient _fakeUserServiceClient;

        public EditDelegatesTests()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _fakeUserServiceClient = A.Fake<IUserServiceClient>();

            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(_fakeUserServiceClient);
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            JSInterop.Setup<Browser>("sfBlazor.Chart.getBrowserDeviceInfo");
        }

        [Fact]
        public void Should_RenderExistingDelegateInTable()
        {
            var userId = Guid.NewGuid();

            A.CallTo(() => _fakeUserServiceClient.All()).Returns(new List<User>
            {
                new() { Id = userId, UserName = "Expected name" }
            });

            var cut = RenderComponent<EditDelegates>(parameters =>
            {
                parameters.Add(p => p.Delegates, new()
                {
                    new() { Subject = userId.ToString(), AccessType = AccessTypes.delegaterw, Reason = "expected reason" },
                });
            });

            var row = cut.WaitForElement("tr.e-row");

            var cells = row.GetElementsByTagName("td");

            Assert.Equal(4, cells.Count());
            Assert.Equal("Expected name", cells[0].FirstChild?.Text());
            Assert.Equal(AccessTypes.delegaterw.ToString(), cells[1].InnerHtml);
            Assert.Equal("expected reason", cells[2].InnerHtml);
        }

        [Fact]
        public void Should_OnlyShowUsersWhoAreNotCurrentlyDelegatesWhenAdding()
        {
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            A.CallTo(() => _fakeUserServiceClient.All()).Returns(new List<User>
            {
                new() { Id = user1Id, UserName = "User 1 name" },
                new() { Id = user2Id, UserName = "User 2 name" }
            });

            var cut = RenderComponent<EditDelegates>(properties =>
            {
                properties.Add(p => p.Delegates, new()
                {
                    new() { Subject = user1Id.ToString(), UserName = "User 1 name" },
                });
            });

            var addButton = cut.Find("[data-testid=add-delegates-button]");

            addButton.Click();

            var usersDropdown = cut.FindComponent<SfMultiSelect<List<string>, UserModel>>();

            Assert.Single(usersDropdown.Instance.DataSource);
            Assert.Equal(user2Id.ToString(), usersDropdown.Instance.DataSource.First().Id);
        }
    }
}
