// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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
