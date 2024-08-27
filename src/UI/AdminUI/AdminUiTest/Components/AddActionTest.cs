using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Google.Api;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Application;
using Siccar.UI.Admin.Models;
using Siccar.UI.Admin.Shared.Components;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdminUiTest.Components
{
    public class AddActionTest : TestContext
    {
        public AddActionTest()
        {
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_RenderExpectedAction_WhenActionIdIsProvided()
        {
            var expected = new Siccar.Application.Action { Id = 2, Title = "SecondAction" };
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action(), expected } };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.ActionId, 2);
                parameters.Add(p => p.Blueprint, blueprint);
            });

            var uiAction = cut.Instance.Action;
            Assert.Equal(expected.Id, uiAction.Id);
        }

        [Fact]
        public void Should_IncrementIdWhenIsNewAction()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action() } };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
            });

            var uiAction = cut.Instance.Action;
            Assert.Equal(blueprint.Actions.Count + 1, uiAction.Id);
        }

        [Fact]
        public async Task Should_RenderAddDataSchemaOnClick()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action() } };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
            });

            cut.Instance.Action = new UIAction()
            {
                DataSchemas = new List<DataSchema>
                {
                    new DataSchema()
                    {
                        Properties = new Dictionary<string, DataSchemaProperty>
                        {
                            {"Data1", new DataSchemaProperty(){ Id = "Data1 ", Title = "Data1Title"} }
                        }
                    }
                }
            };
            cut.Render();

            await cut.InvokeAsync(() =>
            {
                var div = cut.FindAll("div").First(div => div.InnerHtml == "Data1Title");
                div.ParentElement!.Click();
            });

            Assert.NotNull(cut.FindComponent<AddDataSchema>());
        }

        [Fact]
        public async Task Should_RenderAddDataSchema_OnAddDataFieldClick()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new() } };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
            });

            cut.Instance.Action = new UIAction();
            cut.Render();

            await cut.InvokeAsync(() =>
            {
                var addDataFieldButton = cut.FindAll("button").First(b => b.TextContent == "ADD DATA FIELD");
                addDataFieldButton.Click();
            });

            Assert.NotNull(cut.FindComponent<AddDataSchema>());
        }

        [Fact]
        public async Task Should_RenderAddDataSchema_WhenOneDataSchemaAlreadyExitst()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new() } };
            var dataShchemaProperyId = Guid.NewGuid().ToString();
            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
            });

            cut.Instance.Action = new UIAction()
            {
                DataSchemas = new List<DataSchema> { 
                    new DataSchema { Properties = new Dictionary<string, DataSchemaProperty>() { [dataShchemaProperyId] = new DataSchemaProperty() } } }
            };
            cut.Render();

            await cut.InvokeAsync(() =>
            {
                var addDataFieldButton = cut.FindAll("button").First(b => b.TextContent == "ADD DATA FIELD");
                addDataFieldButton.Click();
            });

            Assert.NotNull(cut.FindComponent<AddDataSchema>());
        }

        [Fact]
        public async Task Should_DeleteDataSchemaOnClick()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action() } };
            var uiAction = new UIAction()
            {
                Disclosures = new List<Disclosure> { new Disclosure {
                    ParticipantAddress = "ba0bbaff-19ec-409c-94b4-b1309d19466d",
                    DataPointers = new List<string> { "Data1", "Data2" }
                }},
                DataSchemas = new List<DataSchema>
                {
                    new DataSchema()
                    {
                        Required = new List<string> {"Data1", "Data2"},
                        Properties = new Dictionary<string, DataSchemaProperty>
                        {
                            {"Data1", new DataSchemaProperty(){ Id = "Data1", Title = "Data1Title"} },
                            {"Data2", new DataSchemaProperty(){ Id = "Data2", Title = "Data2Title"} }
                        }
                    }
                }
            };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
                parameters.Add(p => p.SelectedDataSchemaProperty, uiAction.DataSchemas[0].Properties.First().Value);
            });

            cut.Instance.Action = uiAction;
            cut.Render();

            Assert.Equal(2, cut.Instance.Action.DataSchemas[0].Properties.Count);
            Assert.Equal(2, cut.Instance.Action.DataSchemas[0].Required.Count);
            Assert.Equal(2, cut.Instance.Action.Disclosures[0].DataPointers.Count);
            await cut.InvokeAsync(() =>
            {
                var data1DeleteDiv = cut.FindAll("div").First(div => div.ClassList.Contains("blueprint-data-row-delete"));
                data1DeleteDiv.Click();
            });
            Assert.Single(cut.Instance.Action.DataSchemas[0].Properties);
            Assert.Equal("Data2", cut.Instance.Action.DataSchemas[0].Properties["Data2"].Id);
            Assert.Equal("Data2", cut.Instance.Action.DataSchemas[0].Required[0]);
            Assert.Equal("Data2", cut.Instance.Action.Disclosures[0].DataPointers[0]);
            Assert.Null(cut.Instance.SelectedDataSchemaProperty);
        }

        [Fact]
        public async Task ShouldStopRenderingDataSchemaComponentOnSave()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action() } };
            var uiAction = new UIAction()
            {
                Title = "Test",
                Disclosures = new List<Disclosure> { new Disclosure {
                    ParticipantAddress = "ba0bbaff-19ec-409c-94b4-b1309d19466d",
                    DataPointers = new List<string> { "Data1", "Data2" }
                }},
                DataSchemas = new List<DataSchema>
                {
                    new DataSchema()
                    {
                        Required = new List<string> {"Data1"},
                        Properties = new Dictionary<string, DataSchemaProperty>
                        {
                            {"Data1", new DataSchemaProperty(){ Id = "Data1", Title = "Data1Title", Type = ControlTypes.TextLine.ToString()} }
                        }
                    }
                }
            };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.SaveActionHandler, (action) => { });
                parameters.Add(p => p.Blueprint, blueprint);
                parameters.Add(p => p.SelectedDataSchemaProperty, uiAction.DataSchemas[0].Properties.First().Value);
            });
            //Add Action
            cut.Instance.Action = uiAction;
            cut.Render();
            //Select DataSchemaProperty
            await cut.InvokeAsync(() =>
            {
                var div = cut.FindAll("div").First(div => div.InnerHtml == "Data1Title");
                Assert.NotNull(div.ParentElement);
                div.ParentElement.Click();
            });
            cut.Render();
            //Find And Click Save Button
            var dataSchemaForm = cut.FindComponent<AddDataSchema>().Find("form");
            dataSchemaForm.Submit();
            cut.Render();

            Assert.Throws<ComponentNotFoundException>(cut.FindComponent<AddDataSchema>);
            Assert.Null(cut.Instance.SelectedDataSchemaProperty);
            Assert.Null(cut.Instance.SelectedDataSchemaPropertyId);
        }

        [Fact]
        public async Task ShouldStopRenderingDataSchemaComponentOnCancel()
        {
            var blueprint = new Blueprint { Actions = new List<Siccar.Application.Action> { new Siccar.Application.Action() } };
            var uiAction = new UIAction()
            {
                Disclosures = new List<Disclosure> { new Disclosure {
                    ParticipantAddress = "ba0bbaff-19ec-409c-94b4-b1309d19466d",
                    DataPointers = new List<string> { "Data1", "Data2" }
                }},
                DataSchemas = new List<DataSchema>
                {
                    new DataSchema()
                    {
                        Required = new List<string> {"Data1", "Data2"},
                        Properties = new Dictionary<string, DataSchemaProperty>
                        {
                            {"Data1", new DataSchemaProperty(){ Id = "Data1", Title = "Data1Title", Type = ControlTypes.TextLine.ToString()} },
                            {"Data2", new DataSchemaProperty(){ Id = "Data2", Title = "Data2Title"} }
                        }
                    }
                }
            };

            var cut = RenderComponent<AddAction>(parameters =>
            {
                parameters.Add(p => p.Blueprint, blueprint);
                parameters.Add(p => p.SelectedDataSchemaProperty, uiAction.DataSchemas[0].Properties.First().Value);
            });
            //Add Action
            cut.Instance.Action = uiAction;
            cut.Render();
            //Select DataSchemaProperty
            await cut.InvokeAsync(() =>
            {
                var div = cut.FindAll("div").First(div => div.InnerHtml == "Data1Title");
                Assert.NotNull(div.ParentElement);
                div.ParentElement.Click();
                cut.Render();
                //Find And Click Save Button
                var buttons = cut.FindAll("button");
                var saveButton = buttons.First(button => button.InnerHtml.Contains("CANCEL DATA FIELD"));
                saveButton.Click(new());
                cut.Render();

                Assert.Throws<ComponentNotFoundException>(() => cut.FindComponent<AddDataSchema>());
                Assert.Null(cut.Instance.SelectedDataSchemaProperty);
                Assert.Null(cut.Instance.SelectedDataSchemaPropertyId);
            });
        }
    }
}
