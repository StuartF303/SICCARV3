/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

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