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

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating calculations using JSON Logic
    /// </summary>
    public class CalculationBuilder
    {
        private JsonNode? _calculation;

        /// <summary>
        /// Creates an addition calculation
        /// </summary>
        public JsonNode Add(params JsonNode[] operands)
        {
            var array = new JsonArray();
            foreach (var operand in operands)
                array.Add(JsonNode.Parse(operand.ToJsonString()));
            return new JsonObject { ["+"] = array };
        }

        /// <summary>
        /// Creates a subtraction calculation
        /// </summary>
        public JsonNode Subtract(JsonNode left, JsonNode right)
        {
            return new JsonObject
            {
                ["-"] = new JsonArray
                {
                    JsonNode.Parse(left.ToJsonString()),
                    JsonNode.Parse(right.ToJsonString())
                }
            };
        }

        /// <summary>
        /// Creates a multiplication calculation
        /// </summary>
        public JsonNode Multiply(params JsonNode[] operands)
        {
            var array = new JsonArray();
            foreach (var operand in operands)
                array.Add(JsonNode.Parse(operand.ToJsonString()));
            return new JsonObject { ["*"] = array };
        }

        /// <summary>
        /// Creates a division calculation
        /// </summary>
        public JsonNode Divide(JsonNode numerator, JsonNode denominator)
        {
            return new JsonObject
            {
                ["/"] = new JsonArray
                {
                    JsonNode.Parse(numerator.ToJsonString()),
                    JsonNode.Parse(denominator.ToJsonString())
                }
            };
        }

        /// <summary>
        /// Creates a modulo calculation
        /// </summary>
        public JsonNode Modulo(JsonNode left, JsonNode right)
        {
            return new JsonObject
            {
                ["%"] = new JsonArray
                {
                    JsonNode.Parse(left.ToJsonString()),
                    JsonNode.Parse(right.ToJsonString())
                }
            };
        }

        /// <summary>
        /// References a variable from the submitted data
        /// </summary>
        public JsonNode Variable(string name)
        {
            return JsonNode.Parse($"{{\"var\": \"{name}\"}}")!;
        }

        /// <summary>
        /// Creates a constant value
        /// </summary>
        public JsonNode Constant(object value)
        {
            return JsonNode.Parse(JsonSerializer.Serialize(value))!;
        }

        /// <summary>
        /// Sets the calculation expression directly
        /// </summary>
        public CalculationBuilder WithExpression(JsonNode expression)
        {
            _calculation = expression;
            return this;
        }

        internal JsonNode Build()
        {
            if (_calculation == null)
                throw new InvalidOperationException("No calculation expression defined");

            return _calculation;
        }
    }
}
