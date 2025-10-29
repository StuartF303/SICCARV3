// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using FluentValidation;
using Json.More;
using Json.Schema;
using MongoDB.Driver.Core.Servers;
using Siccar.Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
#nullable enable

namespace Siccar.Application.Validation
{
    public class BlueprintValidator : AbstractValidator<Blueprint>
    {
        public BlueprintValidator()
        {
            RuleSet("Publish", () =>
            {
                RuleFor(b => b.Participants).Must(p => p.Count() >= 2).WithMessage("Blueprint: Requires >=2 Participants");
                RuleFor(b => b.Participants).ForEach(p => p.SetValidator(new ParticipantValidator()));
                RuleFor(b => b.Actions).Must(a => a.Count() >= 1).WithMessage("Blueprint: Requires >=1 Actions");
                RuleFor(b => b.Actions).ForEach(a => a.SetValidator(new ActionValidator()));
            });

            RuleFor(b => b.Id).NotEmpty().WithMessage("Blueprint: Requires Id");
            RuleFor(b => b).Must(HaveActionSenderAndRecipientsInListOfBlueprintParticipants)
                .WithMessage("Action sender or additional recipients could not be found in the list of Blueprint participants.");

            RuleForEach(b => b.Actions).Must(HaveSchemaDataFromFieldsInOrBeforeTheCurrentActionForCalculations)
                .WithMessage("Calculations must have data submitted in the current action or a previous action that is disclosed to the calculation sender.");

            RuleForEach(b => b.Actions).Must(HaveSchemaDataWhichIsNumericForCalculations)
                .WithMessage("Calculations must have data fields which are of type integer or number.");
        }

        private bool HaveSchemaDataWhichIsNumericForCalculations(Blueprint blueprint, Action action)
        {
            var listOfDataSchemaFieldsToCheck = GetListOfCalcuationDataFields(action);

            foreach (var blueprintAction in blueprint.Actions)
            {
                if (blueprintAction.Id > action.Id) break;

                var containsDataSchemas = blueprintAction.DataSchemas!.First().RootElement.TryGetProperty("properties", out var dataSchemas);
                if (!containsDataSchemas) return false;

                foreach (var schema in dataSchemas.EnumerateObject())
                {
                    var schemaProperty = JsonDocument.Parse(schema.Value.GetRawText());
                    schemaProperty.RootElement.TryGetProperty("type", out var typeValue);
                    schemaProperty.RootElement.TryGetProperty("$id", out var fieldName);

                    for (int i = 0; i < listOfDataSchemaFieldsToCheck.Count; i++)
                    {
                        if (fieldName.ToString() == listOfDataSchemaFieldsToCheck[i].Item1 && !(typeValue.ToString() == "integer") && !(typeValue.ToString() == "number"))
                        {
                            //If any calculation fields are not of type integer or number it's invalid.
                            return false;
                        }

                        if (fieldName.ToString() == listOfDataSchemaFieldsToCheck[i].Item1 && (typeValue.ToString() == "integer" || typeValue.ToString() == "number"))
                        {
                            listOfDataSchemaFieldsToCheck[i] = (listOfDataSchemaFieldsToCheck[i].Item1, true);
                        }
                    }
                }
            }
            return listOfDataSchemaFieldsToCheck.All(fields => fields.Item2);
        }

        private bool HaveSchemaDataFromFieldsInOrBeforeTheCurrentActionForCalculations(Blueprint blueprint, Action action)
        {
            var listOfDataSchemaFieldsToCheck = GetListOfCalcuationDataFields(action, true);
            var senderId = action.Sender;

            foreach (var blueprintAction in blueprint.Actions)
            {
                if (blueprintAction.Id > action.Id) break;

                foreach (var disclosure in blueprintAction.Disclosures)
                {
                    for (int i = 0; i < listOfDataSchemaFieldsToCheck.Count; i++)
                    {
                        if (disclosure.ParticipantAddress == senderId && disclosure.DataPointers.Contains(listOfDataSchemaFieldsToCheck[i].Item1))
                        {
                            listOfDataSchemaFieldsToCheck[i] = (listOfDataSchemaFieldsToCheck[i].Item1, true);
                        }
                    }
                }
            }

            return listOfDataSchemaFieldsToCheck.All(fields => fields.Item2);
        }

        private List<(string, bool)> GetListOfCalcuationDataFields(Action action, bool disclosureCheck = false)
        {
            var listOfDataSchemaFieldsToCheck = new List<(string, bool)>();
            foreach (var calculation in action.Calculations!)
            {
                var calcString = calculation.Value.Root.AsJsonString();
                var regex = new Regex("{\"var\":\"(.*?)\"}");
                var matches = regex.Matches(calcString);
                foreach (Match match in matches)
                {
                    var value = match.Groups[1].Value;
                    var dataSchemaString = action.DataSchemas!.First().RootElement.GetRawText();
                    if (disclosureCheck && dataSchemaString.Contains($""" "{value}": """))
                    {
                        listOfDataSchemaFieldsToCheck.Add((value, true));
                    }
                    else
                    {
                        listOfDataSchemaFieldsToCheck.Add((value, false));
                    }
                }
            }

            return listOfDataSchemaFieldsToCheck;
        }

        private bool HaveActionSenderAndRecipientsInListOfBlueprintParticipants(Blueprint blueprint)
        {
            var participantIds = blueprint.Participants.Select(p => p.Id);
            return blueprint.Actions.All(action =>
            {
                return participantIds.Contains(action.Sender) && action.AdditionalRecipients.All(recipientId => participantIds.Contains(recipientId));
            });
        }
    }
}
