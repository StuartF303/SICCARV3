using Bunit;
using Bunit.Rendering;
using Bunit.TestDoubles;
using FakeItEasy;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Pages.Blueprint;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using Xunit;
using Siccar.Application;
using System.Threading.Tasks;
using Siccar.Common.Exceptions;
using Siccar.UI.Admin.Services;
using System.Runtime.CompilerServices;
using Syncfusion.Blazor.DropDowns;
using Action = Siccar.Application.Action;

namespace AdminUiTest.Pages.Blueprint
{
    public class CreateBlueprintTest : TestContext
    {
        private readonly IBlueprintServiceClient _fakeBlueprintServiceClient;
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly ITenantServiceClient _fakeTenantServiceClient;
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IValidator<Siccar.Application.Blueprint> _fakeValidator;
        private FakeNavigationManager _navMan;

        public CreateBlueprintTest()
        {
            _fakeBlueprintServiceClient = A.Fake<IBlueprintServiceClient>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _fakeValidator = A.Fake<IValidator<Siccar.Application.Blueprint>>();
            JSInterop.SetupVoid("downloadFileFromStream");
            JSInterop.Mode = JSRuntimeMode.Loose;
            Services.AddSingleton(_fakeValidator);
            Services.AddSingleton(_fakeWalletServiceClient);
            Services.AddSingleton(_fakeBlueprintServiceClient);
            Services.AddSingleton(_fakeRegisterServiceClient);
            Services.AddSingleton(_fakeTenantServiceClient);
            Services.AddSingleton(new PageHistoryState());
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
            _navMan = Services.GetRequiredService<FakeNavigationManager>();
        }

        [Fact]
        public void Should_CallGetPublishedBlueprint_WhenIDParamSet()
        {
            var expectedId = Guid.NewGuid().ToString();
            var expectedRegister = Guid.NewGuid().ToString("N");
            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(A<string>._))
                .Throws(() => new HttpStatusException(System.Net.HttpStatusCode.NotFound, ""));

            _navMan.NavigateTo($"/blueprints/published/{expectedRegister}/{expectedId}");
            RenderComponent<CreateBlueprint>(parameters =>
            {
                parameters.Add(p => p.BlueprintId, expectedId);
                parameters.Add(p => p.RegisterId, expectedRegister);
            }
            );

            A.CallTo(() => _fakeBlueprintServiceClient.GetPublished(expectedRegister, expectedId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_CallGetDraftBlueprint_WhenIDParamSet()
        {
            var expectedId = Guid.NewGuid().ToString();
            _navMan.NavigateTo($"/blueprints/draft/{expectedId}");
            RenderComponent<CreateBlueprint>(parameters =>
            {
                parameters.Add(p => p.BlueprintId, expectedId);
            });

            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(expectedId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_NOT_CallGetBlueprint_WhenIDParamSetEmpty()
        {
            var expectedId = Guid.NewGuid().ToString();

            RenderComponent<CreateBlueprint>();

            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(expectedId)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_SaveBlueprintDraft_OnClick()
        {
            var expectedId = Guid.NewGuid().ToString();

            var componenet = RenderComponent<CreateBlueprint>();

            componenet.Instance.Blueprint.Id = expectedId;
            componenet.Instance.Blueprint.Title = "Title";
            componenet.Instance.Blueprint.Description = "Description";

            var saveButton = componenet.FindAll("button").First(button => button.InnerHtml.Contains("Save"));

            saveButton.Click();
            A.CallTo(() => _fakeBlueprintServiceClient.CreateBlueprintDraft(A<Siccar.Application.Blueprint>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_NavigateToBlueprintId_OnSave()
        {
            var expectedId = Guid.NewGuid().ToString();
            A.CallTo(() => _fakeBlueprintServiceClient.CreateBlueprintDraft(A<Siccar.Application.Blueprint>._)).Returns(new Siccar.Application.Blueprint { Id = expectedId });
            var componenet = RenderComponent<CreateBlueprint>();
            componenet.Instance.Blueprint.Title = "Title";
            componenet.Instance.Blueprint.Description = "Description";

            var saveButton = componenet.FindAll("button").First(button => button.InnerHtml.Contains("Save"));
            saveButton.Click();
            Assert.Equal($"http://localhost/blueprints/draft/{expectedId}", _navMan.Uri);
        }

        [Fact]
        public void Should_UpdateBlueprintDraft_OnClick_WhenBlueprintIdParamSet()
        {
            var expectedId = Guid.NewGuid().ToString();
            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(A<string>._))
                .Returns(Task.FromResult<Siccar.Application.Blueprint>(new()));

            var component = RenderComponent<CreateBlueprint>(parameters => parameters.Add(p => p.BlueprintId, expectedId));

            component.Instance.Blueprint.Title = "Title";
            component.Instance.Blueprint.Description = "Description";

            var saveButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Save Changes"));
            saveButton.Click();
            A.CallTo(() => _fakeBlueprintServiceClient.UpdateBlueprintDraft(A<Siccar.Application.Blueprint>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Export_To_Json_Should_Call_File_Download()
        {
            A.CallTo(() => _fakeBlueprintServiceClient.GetPublished(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new Siccar.Application.Blueprint());
            var component = RenderComponent<CreateBlueprint>(parameters => parameters.Add(p => p.BlueprintId, "test-blueprint"));
            component.InvokeAsync(() =>
            {
                var exportToJsonButton = component.FindAll("button").First(b => b.TextContent.Contains("JSON"));
                exportToJsonButton.Click();
                Assert.Contains(JSInterop.Invocations, i => i.Identifier == "downloadFileFromStream");
            });
        }

        [Fact]
        public void Should_RenderAddActionComponent_OnAddActionClick()
        {
            var component = RenderComponent<CreateBlueprint>();

            var addButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Add Action"));
            addButton.Click();
            component.Render();

            Assert.NotNull(component.FindComponent<AddAction>());
        }

        [Fact]
        public void Should_HideAddActionComponent_OnCancelActionClick()
        {
            var component = RenderComponent<CreateBlueprint>();

            var addButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Add Action"));
            addButton.Click();
            component.Render();
            var cancelButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Cancel Action"));
            cancelButton.Click();
            component.Render();

            Assert.Throws<ComponentNotFoundException>(() => component.FindComponent<AddAction>());
            Assert.Equal(0, component.Instance.SelectedActionId);
        }

        [Fact]
        public void Should_SaveAction_OnClick()
        {
            var component = RenderComponent<CreateBlueprint>();
            component.Instance.Blueprint.Actions = new List<Action>
            {
                new()
                {
                    Id = 1,
                    Title = "test"
                }
            };
            var addButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Add Action"));
            addButton.Click();
            var addActionCp = component.FindComponent<AddAction>();
            addActionCp.Instance.Action.Title = "Test";
            component.Render();
            var saveButton = component.FindAll("button").First(button => button.InnerHtml.Contains("Save Action"));
            saveButton.Click();
            component.Render();

            var listBox = component.FindComponent<SfListBox<int, Action>>();
            var actionTitle = listBox.FindAll("div").First(div => div.ClassList.Contains("blueprint-action-row-title"));
            Assert.NotNull(actionTitle);
            Assert.Throws<ComponentNotFoundException>(() => component.FindComponent<AddAction>());
            Assert.Equal(0, component.Instance.SelectedActionId);
        }

        [Fact]
        public void Should_RenderExistingAction_OnClick()
        {
            var blueprint = new Siccar.Application.Blueprint { Id = Guid.NewGuid().ToString(), Actions = new() { new() { Title = "Test Action" } } };
            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(blueprint.Id)).Returns(blueprint);

            var component = RenderComponent<CreateBlueprint>(parameters => parameters.Add(p => p.BlueprintId, blueprint.Id));

            var actionTitle = component.FindAll("div").First(div => div.ClassList.Contains("blueprint-action-row-title"));
            Assert.Contains(blueprint.Actions[0].Title, actionTitle.InnerHtml);
            Assert.NotNull(actionTitle.ParentElement);
            actionTitle.ParentElement.Click();

            Assert.NotNull(component.FindComponent<AddAction>());
        }

        [Fact]
        public void Should_Apply_SelectedAction_On_ActionClick()
        {
            var blueprint = new Siccar.Application.Blueprint
            {
                Id = Guid.NewGuid().ToString(),
                Actions = new()
                {
                    new() { Id = 1, Title = "Test Action" },
                    new() { Id = 2, Title = "Test Action 2" },
                    new() { Id = 3, Title = "Test Action 3" }
                }
            };
            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(blueprint.Id)).Returns(blueprint);

            var component = RenderComponent<CreateBlueprint>(parameters => parameters.Add(p => p.BlueprintId, blueprint.Id));

            var actionTitle = component.FindAll("div").First(div => div.ClassList.Contains("blueprint-action-row-title"));
            Assert.Contains(blueprint.Actions[0].Title, actionTitle.InnerHtml);
            Assert.NotNull(actionTitle.ParentElement);
            actionTitle.ParentElement.Click();

            var selectedActionCount = component.FindAll("div").Count(div => div.ClassList.Any(c => c.Contains("action-row-selected-True")));
            var unselectedActionCount = component.FindAll("div").Count(div => div.ClassList.Any(c => c.Contains("action-row-selected-False")));

            Assert.Equal(1, selectedActionCount);
            Assert.Equal(2, unselectedActionCount);
        }

        [Fact]
        public async Task  Should_Call_PublishBlueprintWhenPublishClicked()
        {
            var expectedRegister = Guid.NewGuid().ToString("N");
            var blueprint = new Siccar.Application.Blueprint
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Title",
                Description = "Test BP",
                Actions = new()
                {
                    new() { Id = 1, Title = "Test Action" },
                }
            };

            A.CallTo(() => _fakeBlueprintServiceClient.GetPublished(A<string>.Ignored, A<string>.Ignored)).Returns(blueprint);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            //TODO - When service clients responses are handled properly fix this.
            A.CallTo(() => _fakeBlueprintServiceClient.GetBlueprintDraft(A<string>.Ignored)).Returns<Siccar.Application.Blueprint>(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            _navMan.NavigateTo($"/blueprints/published/{expectedRegister}/{blueprint.Id}");
            var component = RenderComponent<CreateBlueprint>(parameters =>
            {
                parameters.Add(p => p.BlueprintId, blueprint.Id);
                parameters.Add(p => p.RegisterId, expectedRegister);
            });
            component.Render();

            await component.InvokeAsync(() =>
            {
                var form = component.Find("form");
                form.Submit();
            });

            A.CallTo(() => _fakeBlueprintServiceClient
            .PublishBlueprint(A<string>._, A<string>._, A<Siccar.Application.Blueprint>.That.Matches(bp => bp.Id == blueprint.Id))).MustHaveHappenedOnceExactly();
        }
    }
}
