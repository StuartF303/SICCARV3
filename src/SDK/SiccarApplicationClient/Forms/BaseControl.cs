// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Siccar.Application.Client.Schema;
using Json.Schema;
using Json.Path;
using Siccar.Application.Client.Services;
using System.Diagnostics;
using SiccarAction = Siccar.Application.Action;
using System.Text.Json.Nodes;
using Json.More;

namespace Siccar.Application.Client.Forms
{
    /// <summary>
    /// Base Control - Backing logic for a Control Element to Read its properties from a JSON Schema Defintion
    /// </summary>
    public class BaseControl : ComponentBase
    {
        [Inject]
        public DataSubmissionService dataService { get; set; }

        [Inject]
        public HeroDemoServicesLogic heroDemoServicesLogic { get; set; }

        [CascadingParameter(Name = "Action")]
        public Siccar.Application.Action action { get; set; }

        [Parameter]
        public System.Action<SiccarAction> updateParentState { get; set; }

        [Parameter]
        public Siccar.Application.Control control { get; set; }

        [CascadingParameter(Name = "WalletAddress")]
        public string WalletAddress { get; set; }

        [CascadingParameter(Name = "RegisterId")]
        public string RegisterId { get; set; }

        public string _id;
        public string _type = "string";
        public string _label = "label";
        public string _placeholder = "placeholder";
        public string _description = "description";
        public List<string> _choiceEnum = new();
        public string _validation = "*.";
        public bool _hidden = false;
        public bool _required = false;
        public bool _disabled = false;

        public List<string> _options = new();


        protected override Task OnInitializedAsync()
        {
            heroDemoServicesLogic.UpdateForm(action.Id);

            if ((action is not null) && (control is not null))
            {

                // probably some better validation here
                if (control.Scope is null)
                    control.Scope = string.Empty;

                if (action.DataSchemas is null)
                    action.DataSchemas = new List<JsonDocument>() { JsonDocument.Parse("{}") };

                ProcessDataField(action.DataSchemas.First(), control.Scope);

            }
            else
            {
                Console.WriteLine("ACTION CONTROL CANNOT BE NULL (Init)");
            }
            return base.OnInitializedAsync();
        }

        protected override Task OnParametersSetAsync()
        {
            if ((action is not null) && (control is not null))
            {
                ProcessDataField(action.DataSchemas.First(), control.Scope);

                //if (control.Conditions is not null)
                //{
                //    var conditionResult = ProcessCondition(control.Conditions);
                //    //_hidden = !conditionResult;
                //}
            }
            else
            {
                Console.WriteLine("ACTION CONTROL CANNOT BE NULL (Params)");
            }
            StateHasChanged();
            return base.OnParametersSetAsync();
        }

        public bool ProcessCondition(List<JsonDocument> condition)
        {
            // evaluate the Conditions and return true or false
            // only handle first condition at the moment
            var conditionString = condition[0].RootElement.GetRawText();
            var rule = JsonSerializer.Deserialize<Json.Logic.Rule>(conditionString);

            JsonNode data = JsonNode.Parse(dataService.GetFormDataAsJson().RootElement.GetRawText());
            var result = rule.Apply(data);
            // right need to work out how to evaluate this to a true or false..

            var boolstring = JsonSerializer.Deserialize<string>(result.Root.ToString());
            return bool.Parse(boolstring);
        }

        public void ProcessDataField(JsonDocument dataSchema, string scope)
        {
            if ((!string.IsNullOrEmpty(scope)) && (dataSchema is not null))
            {
                Debug.WriteLine("BaseControl: Getting Data Shape of {0}", scope);

                var pathOut = JsonPath.Parse(scope); // get a path from the scope

                PathResult fieldDetails = pathOut.Evaluate(dataSchema.RootElement.AsNode());

                if (fieldDetails.Matches.Count > 0)
                {
                    var fieldSchema = JsonSchema.FromText(fieldDetails.Matches[0].Value.ToString());
                    var schemaParser = new SchemaParser(fieldSchema);
                    _id = schemaParser.GetField("$id");
                    _type = schemaParser.GetField("type") ?? "string";
                    _label = schemaParser.GetField("title") ?? "field_title";
                    _description = schemaParser.GetField("description") ?? "field_description";
                    _placeholder = schemaParser.GetField("example") ?? "field_example";
                    _validation = schemaParser.GetField("pattern") ?? "";
                    _choiceEnum = schemaParser.GetEnum();

                    dynamic _default = schemaParser.GetField("default") ?? string.Empty;

                    //This populates the _default with the previous data if it exists
                    if (dataService.GetFormDataByKey(_id) != null)
                    {
                        _default = dataService.GetFormDataByKey(_id);
                    }

                    if (control.ControlType != ControlTypes.Layout && control.ControlType != ControlTypes.Label)
                    {
                        if ((!string.IsNullOrEmpty(Convert.ToString(_default))) && (_type.ToLower() == "integer"))
                        {
                            decimal dec = decimal.Parse(Convert.ToString(_default));
                            dataService.UpdateFormData(_id, dec);
                        }
                        else if (_type.ToLower() == "boolean")
                        {
                            bool boolean = string.IsNullOrEmpty(Convert.ToString(_default)) ? false : bool.Parse(Convert.ToString(_default));
                            dataService.UpdateFormData(_id, boolean);
                        }
                        else if(_type.ToLower() == "object")
                        {
                            //do nothing
                        }
                        else
                            dataService.UpdateFormData(_id, _default);
                    }

                }
            }
            else
            {
                Debug.WriteLine("BaseControl: Cant Process Scope : '{0}'", scope);
            }
        }
    }
}

