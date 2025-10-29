// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Json.Schema;
using System.Text.Json.Nodes;
#nullable enable

namespace Siccar.Application.Validation
{
    public class SchemaDataValidator : ISchemaDataValidator
    {
        public (bool isValid, string validationMessage) ValidateSchemaData(string schemaString, string data)
        {
            var schema = JsonSchema.FromText(schemaString);
            JsonNode? jdata = JsonNode.Parse(data);
            var validationResult = schema.Evaluate(jdata, new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });

            var message = GetValidationResultMessage(validationResult);
            return (validationResult.IsValid, message);
        }

        private static string GetValidationResultMessage(EvaluationResults validationResult)
        {
            string resultMessage;
            if (validationResult.IsValid)
                resultMessage = "Validation Succeeded.";
            else if (validationResult.HasDetails)
            {
                resultMessage = "Validation Errors: ";
                foreach (var result in validationResult.Details)
                {
                    resultMessage += $"{result.GetAllAnnotations} found at {result.SchemaLocation}| ";
                }
            }
            else
                resultMessage = $"Validation Errors: {validationResult.GetAllAnnotations} found at {validationResult.SchemaLocation.OriginalString}| ";
 
            return resultMessage;
        }
    }
}