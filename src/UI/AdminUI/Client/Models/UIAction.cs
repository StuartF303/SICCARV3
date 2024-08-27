using Siccar.Application;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Action = Siccar.Application.Action;
using System;
using Microsoft.IdentityModel.Tokens;

#nullable enable
namespace Siccar.UI.Admin.Models
{
    public class UIAction
    {
        public UIAction()
        {
        }
        public UIAction(Action action)
        {
            Id = action.Id;
            Blueprint = action.Blueprint;
            Title = action.Title;
            Description = action.Description;
            Sender = action.Sender;
            RequiredActionData = action.RequiredActionData.ToList();
            AdditionalRecipients = action.AdditionalRecipients.ToList();
            Disclosures = action.Disclosures.ToList();
            Condition = action.Condition;
            Form = action.Form!.Elements.IsNullOrEmpty() ? InitialiseControl() : action.Form;
            if (action.DataSchemas != null && action.DataSchemas.Any())
            {
                var dataSchema = JsonSerializer.Deserialize<DataSchema>(action.DataSchemas.First(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
                if (dataSchema != null)
                {
                    DataSchemas = new List<DataSchema> { dataSchema };
                }

                // Set control type (where we can because they might be missing)
                foreach (var property in DataSchemas.First().Properties)
                {
                    if (action.Form!.Elements.Any())
                    {
                        var control =
                            action.Form!.Elements.FirstOrDefault()!.Elements!.FirstOrDefault(e => e.Scope == $"$.properties.{property.Key}");

                        if (control != null)
                        {
                            property.Value.Type = control.ControlType.ToString();
                        }
                    }
                }
                // Set Required property of each DataSchemaProperty
                foreach (var requiredData in DataSchemas.First().Required)
                {
                    var dataSchemaProperty = DataSchemas.First().Properties[requiredData].Required = true;
                }

                foreach (var disclosure in Disclosures)
                {
                    foreach (var property in DataSchemas.First().Properties)
                    {
                        if (disclosure.DataPointers.Contains(property.Key))
                        {
                            property.Value.Disclosures.Add(disclosure.ParticipantAddress);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The Action ID is the Transaction ID that contains it 
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; } = 1;

        /// <summary>
        /// The previous TxId that was used to generate this action
        /// </summary>
        [JsonPropertyName("previousTxId")]
        [MaxLength(64)]
        public string PreviousTxId { get; set; } = string.Empty;

        /// <summary>
        /// Blueprint TX Id this action belongs to
        /// This maintains the 
        /// </summary>
        [JsonPropertyName("blueprint")]
        [MaxLength(64)]
        public string Blueprint { get; set; } = string.Empty;

        /// <summary>
        /// A useful title for this Action i.e. Apply, Endorse
        /// </summary>
        [JsonPropertyName("title")]
        [MaxLength(50)]
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Usefull words for this Action i.e. Your applciation will require the following data...
        /// </summary>
        [JsonPropertyName("description")]
        [MaxLength(2048)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Address of the sender, which may be a stealth/derived addres
        /// </summary>
        [JsonPropertyName("sender")]
        [MaxLength(64)]
        public string Sender { get; set; } = string.Empty;


        /// <summary>
        /// A list of recipients that should exlude the sender of the resolved next action
        /// A string is the ID of the participant to send the transaction to.
        /// </summary>
        public List<string> RequiredActionData { get; set; } = new List<string>();

        /// <summary>
        /// A list of recipients that should exlude the sender of the resolved next action
        /// A string is the ID of the participant to send the transaction to.
        /// </summary>
        public List<string> AdditionalRecipients { get; set; } = new List<string>();

        /// <summary>
        /// List of Disclosure ID's associated with SenderID
        /// Dictionary is DisclosureID an JSONLogic rule
        /// The principal here is the Data Item to be released
        /// Disclosure is under what conditions they get the dates  
        /// </summary>
        public List<Disclosure> Disclosures { get; set; } = new List<Disclosure>();


        /// <summary>
        /// JSON of Data Elements to Pass to the next Participant
        /// </summary>
        //public JsonDocument SubmittedData { get; set; }

        /// <summary>
        /// JSONSchema of Data Elements to Pass to the next Participant
        /// </summary>
        public List<DataSchema>? DataSchemas { get; set; } = new List<DataSchema>();

        /// <summary>
        /// An Action Condition resolves to a number which is the next Action
        /// Evaluated near the end by Json.Logic
        /// </summary>
        public JsonNode? Condition { get; set; } = "{ \"==\":[0,0]}";

        /// <summary>
        /// Specifies the format of the data presentation
        /// </summary>
        public Control? Form { get; set; } = InitialiseControl();

        private static Control InitialiseControl()
        {
            return new Control()
            {
                ControlType = ControlTypes.Layout,
                Layout = LayoutTypes.Group,
                Scope = "$",
                Elements = new List<Control> {
                new Control {
                    ControlType = ControlTypes.Layout,
                    Layout = LayoutTypes.VerticalLayout,
                    Elements = new List<Control>()
                }
            }};
        }

        public void SetPropertyTypesFromControlType()
        {
            if (DataSchemas!.FirstOrDefault() != null)
            {
                foreach (var property in DataSchemas!.First().Properties)
                {
                    property.Value.Type = property.Value.PropertyType;
                }
            }
        }

        public void UpdateSchemaAndRelatedProperties(DataSchemaProperty dataSchemaProperty, string? orignalId)
        {
            if (dataSchemaProperty == null)
                return;

            var idHasChanged = orignalId != null && orignalId != dataSchemaProperty.Id;

            if (DataSchemas == null || !DataSchemas.Any())
            {
                DataSchemas = new List<DataSchema> { new DataSchema() };
            }
            //Remove old property id when updating
            if (idHasChanged)
            {
                DataSchemas[0].Properties.Remove(orignalId!);
                Disclosures.ForEach(dis => dis.DataPointers.Remove(orignalId!));
            }
            DataSchemas[0].Properties[dataSchemaProperty.Id] = dataSchemaProperty;

            //Update required property
            if (dataSchemaProperty.Required && !DataSchemas[0].Required.Contains(dataSchemaProperty.Id))
                DataSchemas[0].Required.Add(dataSchemaProperty.Id);
            else if (!dataSchemaProperty.Required)
                DataSchemas[0].Required.Remove(dataSchemaProperty.Id);

            //Update disclosures
            //Remove All Disclosures
            for (int i = Disclosures.Count - 1; i >= 0; i--)
            {
                Disclosures[i].DataPointers.Remove(dataSchemaProperty.Id);
                if (Disclosures[i].DataPointers.Count < 1)
                    Disclosures.RemoveAt(i);

            }
            Disclosures.ForEach(disclosure =>
            {
                disclosure.DataPointers.Remove(dataSchemaProperty.Id);
            });
            //AddDisclosuresBack
            dataSchemaProperty.Disclosures.ForEach(disclosure =>
            {
                var actionDisclosure = Disclosures.Find(dis => dis.ParticipantAddress == disclosure);
                if (actionDisclosure == null)
                {
                    Disclosures.Add(new Disclosure
                    {
                        ParticipantAddress = disclosure,
                        DataPointers = new List<string>
                                                { dataSchemaProperty.Id }
                    });
                }
                else if (!actionDisclosure.DataPointers.Contains(dataSchemaProperty.Id))
                {
                    actionDisclosure.DataPointers.Add(dataSchemaProperty.Id);

                }
            });

            //Update form
            var dataSchemaElements = Form!.Elements.First().Elements;
            Control dataSchemaControl;
            //Check if the id is the same as the original or if it's changed.
            if (idHasChanged)
            {
                dataSchemaControl = dataSchemaElements!.Find(element => element.Scope.Contains(orignalId!))!;
            }
            else
            {
                dataSchemaControl = dataSchemaElements!.Find(element => element.Scope.Contains(dataSchemaProperty.Id))!;
            }
            //If there is no data type set short circuit before form is added.
            if (dataSchemaControl == null && dataSchemaProperty.Type == null) return;
            //Add control else update existing control
            if (dataSchemaControl == null)
            {
                dataSchemaElements.Add(new Control
                {
                    Scope = $"$.properties.{dataSchemaProperty.Id}",
                    ControlType = Enum.Parse<ControlTypes> (dataSchemaProperty.Type!),
                    Layout = LayoutTypes.VerticalLayout
                });
            }
            else
            {
                dataSchemaControl.Scope = $"$.properties.{dataSchemaProperty.Id}";
                dataSchemaControl.ControlType = Enum.Parse<ControlTypes>
                    (dataSchemaProperty.Type!);

            }
        }
    }
}
