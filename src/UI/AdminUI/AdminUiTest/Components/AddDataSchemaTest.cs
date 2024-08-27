using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using System.Linq;
using Siccar.UI.Admin.Shared.Components;
using Xunit;
using System.Collections.Generic;
using Siccar.Application;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DropDowns;
using static Siccar.UI.Admin.Shared.Components.AddDataSchema;
using AngleSharp.Dom;
using System.Threading.Tasks;
using Siccar.UI.Admin.Models;
using Syncfusion.Blazor.Inputs;
using InputType = Siccar.UI.Admin.Shared.Components.AddDataSchema.InputType;
using Google.Type;
using System.Threading.Channels;

namespace AdminUiTest.Components
{
    public class AddDataSchemaTest : TestContext
    {

        public AddDataSchemaTest()
        {
            Services.AddSyncfusionBlazor();
            Services.AddOptions();
        }

        [Fact]
        public void Should_RenderAllPartcipantsForDisclosures()
        {
            var participants = new List<Participant>{ new Participant
            {
                Id = "fc08c4e4-b34e-444c-ba9f-109ccee94dd8",
                WalletAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj",
                Name = "Operator",
                Organisation = "Oil and Gas Operator"
            },
             new Participant {
                Id = "gh08c4e4-b34e-444c-ba9f-109ccee94dgw5",
                WalletAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj",
                Name = "AnchorHandling",
                Organisation = "Oil and Gas Operator"
            }};

            var cut = RenderComponent<AddDataSchema>(parameters => parameters.Add(p => p.Participants, participants));

            var participantLabels = cut.FindAll("label").Where(b => b.InnerHtml.Contains(participants[0].Name) || b.InnerHtml.Contains(participants[1].Name));

            Assert.True(participantLabels.Count() == participants.Count);
        }

        [Fact]
        public void Should_AddAndRemoveParticipantToDisclosuresOnClick()
        {
            var participants = new List<Participant>{ new Participant
            {
                Id = "fc08c4e4-b34e-444c-ba9f-109ccee94dd8",
                WalletAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj",
                Name = "Operator",
                Organisation = "Oil and Gas Operator"
            },
             new Participant {
                Id = "gh08c4e4-b34e-444c-ba9f-109ccee94dgw5",
                WalletAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj",
                Name = "AnchorHandling",
                Organisation = "Oil and Gas Operator"
            }};

            var cut = RenderComponent<AddDataSchema>(parameters => parameters.Add(p => p.Participants, participants));

            Assert.Empty(cut.Instance.DataSchema.Disclosures);

            var participantSwitch = cut.FindAll("input").First(input => input.Id == participants[0].Name);
            participantSwitch.Click();
            Assert.Single(cut.Instance.DataSchema.Disclosures);

            var participantSwitch2 = cut.FindAll("input").First(input => input.Id == participants[1].Name);
            participantSwitch2.Click();
            participantSwitch2 = cut.FindAll("input").First(input => input.Id == participants[1].Name);
            Assert.True(participantSwitch2.HasAttribute("checked"));
            Assert.Equal(2, cut.Instance.DataSchema.Disclosures.Count);

            //Remove 1st participant
            participantSwitch = cut.FindAll("input").First(input => input.Id == participants[0].Name);
            participantSwitch.Click();
            Assert.Contains(participants[1].Id, cut.Instance.DataSchema.Disclosures);
            Assert.Single(cut.Instance.DataSchema.Disclosures);
        }

        [Fact]
        public async Task Should_RenderAddOptionButtonWhenControlType_IsChoice()
        {
            var cut = RenderComponent<AddDataSchema>();

            var liCollection = await GetDropdownList(cut);
            liCollection.First(li => li.InnerHtml.Contains("Multiple Choice")).Click();

            var addOptionButton = cut.FindAll("button").First(input => input.TextContent.Contains("Add Option"));
            Assert.NotNull(addOptionButton);
        }

        [Fact]
        public async Task Should_AddOptionsToDataSchema()
        {
            var cut = RenderComponent<AddDataSchema>();

            var liCollection = await GetDropdownList(cut);
            liCollection.First(li => li.InnerHtml.Contains("Multiple Choice")).Click();

            var addOptionButton = cut.FindAll("button").First(input => input.TextContent.Contains("Add Option"));
            Assert.NotNull(cut.Instance.DataSchema.Enum);
            Assert.Empty(cut.Instance.DataSchema.Enum);
            addOptionButton.Click();
            Assert.Single(cut.Instance.DataSchema.Enum);
            addOptionButton.Click();
            Assert.Equal(2, cut.Instance.DataSchema.Enum.Count);
        }

        [Fact]
        public async Task Should_RemoveOptionFromDataSchema()
        {
            var cut = RenderComponent<AddDataSchema>();

            var liCollection = await GetDropdownList(cut);
            liCollection.First(li => li.InnerHtml.Contains("Multiple Choice")).Click();

            var addOptionButton = cut.FindAll("button").First(input => input.TextContent.Contains("Add Option"));
            Assert.NotNull(cut.Instance.DataSchema.Enum);
            addOptionButton.Click();
            var deleteOptionDiv = cut.FindAll("div").First(div => div.ClassList.Contains("row-delete"));
            deleteOptionDiv.Click();
            Assert.Empty(cut.Instance.DataSchema.Enum);
        }

        [Fact]
        public async Task Should_EditOptionTextWhenChanged()
        {
            var expected = "New Option Data";
            var cut = RenderComponent<AddDataSchema>();

            var liCollection = await GetDropdownList(cut);
            liCollection.First(li => li.InnerHtml.Contains("Multiple Choice")).Click();

            var addOptionButton = cut.FindAll("button").First(input => input.TextContent.Contains("Add Option"));
            Assert.NotNull(cut.Instance.DataSchema.Enum);
            Assert.Empty(cut.Instance.DataSchema.Enum);
            addOptionButton.Click();

            var optionInput = cut.FindAll("input").First(input => input.GetAttribute("placeholder") == "Enter Option Text");
            optionInput.Change(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = expected });
            Assert.Equal(expected, cut.Instance.DataSchema.Enum[0]);
        }

        [Fact]
        public void Should_RenderEnumOptions_WhenTypeIsChoice()
        {
            var dataSchema = new DataSchemaProperty()
            {
                Type = ControlTypes.Choice.ToString(),
                Enum = new List<string> { "Enum1", "Enum2" }
            };
            var cut = RenderComponent<AddDataSchema>(parameters => parameters.Add(p => p.DataSchema, dataSchema));

            var textBoxes = cut.FindComponents<SfTextBox>();

            var enum1 = textBoxes.First(textbox => textbox.Instance.Value == dataSchema.Enum[0]);
            var enum2 = textBoxes.First(textbox => textbox.Instance.Value == dataSchema.Enum[0]);
            Assert.NotNull(enum1);
            Assert.NotNull(enum2);
        }

        [Fact]
        public void Should_InvokeSavelHandlerOnClick()
        {
            var expected = new DataSchemaProperty() { Id = "test", Type = ControlTypes.TextLine.ToString() };
            DataSchemaProperty? returnedSchema = null;
            void SaveDataSchemaHandler(DataSchemaProperty dataSchemaProperty)
            {
                returnedSchema = dataSchemaProperty;
            };

            var cut = RenderComponent<AddDataSchema>(parameters =>
            {
                parameters.Add(p => p.DataSchema, expected);
                parameters.Add(p => p.AddDataSchemaHandler, SaveDataSchemaHandler);
            });

            cut.InvokeAsync(() =>
            {
                cut.Find("form").Submit();
                Assert.Equal(expected, returnedSchema);
            });
        }

        [Fact]
        public void Should_InvokeCancelHandlerOnClick()
        {
            var wasCancelled = false;
            var cancelAction = () => { wasCancelled = true; };

            var cut = RenderComponent<AddDataSchema>(parameters => parameters.Add(p => p.CancelAddDataSchema, cancelAction));

            var buttons = cut.FindAll("button");
            var cancelButton = buttons.First(button => button.InnerHtml.Contains("CANCEL DATA FIELD"));
            cancelButton.Click();
            Assert.True(wasCancelled);
        }


        private static async Task<IHtmlCollection<IElement>> GetDropdownList(IRenderedComponent<AddDataSchema> cut)
        {
            var dropdownList = cut.FindComponent<SfDropDownList<string, InputType>>();
            await dropdownList.Instance.ShowPopup();
            Assert.NotNull(dropdownList);
            var popupEle = dropdownList.Find(".e-popup");
            Assert.Contains("e-popup", popupEle.ClassName);
            var ulEle = popupEle.QuerySelector("ul");
            Assert.NotNull(ulEle);
            Assert.Contains("e-ul", ulEle.ClassName);
            var liCollection = ulEle.QuerySelectorAll("li.e-list-item");
            return liCollection;
        }
    }
}
