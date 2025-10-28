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
using Siccar.Application;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating conditional routing logic using JSON Logic
    /// </summary>
    public class ConditionBuilder
    {
        private readonly List<string> _participantIds;
        private readonly List<ConditionalRoute> _routes;

        internal ConditionBuilder(List<string> participantIds)
        {
            _participantIds = participantIds;
            _routes = new List<ConditionalRoute>();
        }

        /// <summary>
        /// Defines a condition using JSON Logic
        /// </summary>
        /// <param name="condition">Function that builds the JSON Logic condition</param>
        /// <returns>The builder instance for chaining</returns>
        public ConditionBuilder When(Func<JsonLogicBuilder, JsonNode> condition)
        {
            var logicBuilder = new JsonLogicBuilder();
            var jsonLogic = condition(logicBuilder);

            _routes.Add(new ConditionalRoute
            {
                Condition = jsonLogic.ToJsonString()
            });

            return this;
        }

        /// <summary>
        /// Specifies the participant to route to when the condition is true
        /// </summary>
        /// <param name="participantId">The participant ID</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="InvalidOperationException">Thrown if When() was not called first</exception>
        /// <exception cref="ArgumentException">Thrown if participant ID not found</exception>
        public ConditionBuilder ThenRoute(string participantId)
        {
            if (_routes.Count == 0)
                throw new InvalidOperationException("Must call When() before ThenRoute()");

            if (!_participantIds.Contains(participantId))
                throw new ArgumentException($"Participant '{participantId}' not found in blueprint");

            _routes[_routes.Count - 1].Participant = participantId;
            return this;
        }

        /// <summary>
        /// Specifies the default participant to route to when no conditions are true
        /// </summary>
        /// <param name="participantId">The participant ID</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown if participant ID not found</exception>
        public ConditionBuilder ElseRoute(string participantId)
        {
            if (!_participantIds.Contains(participantId))
                throw new ArgumentException($"Participant '{participantId}' not found in blueprint");

            _routes.Add(new ConditionalRoute
            {
                Condition = "{\"!\":[false]}", // Always true
                Participant = participantId
            });

            return this;
        }

        internal IEnumerable<Condition> BuildParticipantConditions()
        {
            var conditions = new List<Condition>();

            foreach (var route in _routes)
            {
                conditions.Add(new Condition(
                    route.Participant,
                    new List<string> { route.Condition }));
            }

            return conditions;
        }

        internal JsonNode BuildActionCondition()
        {
            // Build next action determination logic
            // For now, simple equality (same action continues)
            return JsonNode.Parse("{\"==\":[0,0]}")!;
        }

        private class ConditionalRoute
        {
            public string Condition { get; set; } = string.Empty;
            public string Participant { get; set; } = string.Empty;
        }
    }

    /// <summary>
    /// Helper builder for creating JSON Logic expressions
    /// </summary>
    public class JsonLogicBuilder
    {
        /// <summary>
        /// Creates a greater-than comparison
        /// </summary>
        public JsonNode GreaterThan(string variable, object value)
        {
            return JsonNode.Parse($"{{\">\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates a greater-than-or-equal comparison
        /// </summary>
        public JsonNode GreaterThanOrEqual(string variable, object value)
        {
            return JsonNode.Parse($"{{\">=\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates a less-than comparison
        /// </summary>
        public JsonNode LessThan(string variable, object value)
        {
            return JsonNode.Parse($"{{\"<\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates a less-than-or-equal comparison
        /// </summary>
        public JsonNode LessThanOrEqual(string variable, object value)
        {
            return JsonNode.Parse($"{{\"<=\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates an equality comparison
        /// </summary>
        public JsonNode Equals(string variable, object value)
        {
            return JsonNode.Parse($"{{\"==\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates a not-equal comparison
        /// </summary>
        public JsonNode NotEquals(string variable, object value)
        {
            return JsonNode.Parse($"{{\"!=\": [{{\"var\": \"{variable}\"}}, {JsonSerializer.Serialize(value)}]}}")!;
        }

        /// <summary>
        /// Creates a logical AND of multiple conditions
        /// </summary>
        public JsonNode And(params JsonNode[] conditions)
        {
            var array = new JsonArray();
            foreach (var condition in conditions)
                array.Add(JsonNode.Parse(condition.ToJsonString()));

            return new JsonObject { ["and"] = array };
        }

        /// <summary>
        /// Creates a logical OR of multiple conditions
        /// </summary>
        public JsonNode Or(params JsonNode[] conditions)
        {
            var array = new JsonArray();
            foreach (var condition in conditions)
                array.Add(JsonNode.Parse(condition.ToJsonString()));

            return new JsonObject { ["or"] = array };
        }

        /// <summary>
        /// Creates a logical NOT of a condition
        /// </summary>
        public JsonNode Not(JsonNode condition)
        {
            return new JsonObject { ["!"] = new JsonArray { JsonNode.Parse(condition.ToJsonString()) } };
        }

        /// <summary>
        /// References a variable in the data
        /// </summary>
        public JsonNode Variable(string name)
        {
            return JsonNode.Parse($"{{\"var\": \"{name}\"}}")!;
        }
    }
}
