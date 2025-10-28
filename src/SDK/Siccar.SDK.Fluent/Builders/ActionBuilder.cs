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
using Siccar.Application.Constants;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating Blueprint actions
    /// </summary>
    public class ActionBuilder
    {
        private readonly Siccar.Application.Action _action;
        private readonly List<string> _participantIds;
        private DataSchemaBuilder? _schemaBuilder;
        private readonly List<Disclosure> _disclosures;
        private readonly Dictionary<string, JsonNode> _calculations;
        private FormBuilder? _formBuilder;

        internal ActionBuilder(string actionId, List<string> participantIds, int numericId)
        {
            _participantIds = participantIds;
            _action = new Siccar.Application.Action
            {
                Id = numericId,
                Blueprint = string.Empty,
                PreviousTxId = string.Empty,
                AdditionalRecipients = new List<string>(),
                RequiredActionData = new List<string>(),
                Calculations = new Dictionary<string, JsonNode>()
            };
            _disclosures = new List<Disclosure>();
            _calculations = new Dictionary<string, JsonNode>();
        }

        /// <summary>
        /// Sets the action title
        /// </summary>
        /// <param name="title">The action title</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder WithTitle(string title)
        {
            _action.Title = title;
            return this;
        }

        /// <summary>
        /// Sets the action description
        /// </summary>
        /// <param name="description">The action description</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder WithDescription(string description)
        {
            _action.Description = description;
            return this;
        }

        /// <summary>
        /// Specifies which participant sends this action
        /// </summary>
        /// <param name="participantId">The participant ID</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown if participant ID not found</exception>
        public ActionBuilder SentBy(string participantId)
        {
            if (!_participantIds.Contains(participantId))
                throw new ArgumentException($"Participant '{participantId}' not found in blueprint");

            _action.Sender = participantId;
            return this;
        }

        /// <summary>
        /// Defines the data schema for this action
        /// </summary>
        /// <param name="configure">Configuration action for the data schema</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder RequiresData(Action<DataSchemaBuilder> configure)
        {
            _schemaBuilder = new DataSchemaBuilder();
            configure(_schemaBuilder);
            return this;
        }

        /// <summary>
        /// Defines selective data disclosure for a participant
        /// </summary>
        /// <param name="participantId">The participant ID to disclose data to</param>
        /// <param name="configure">Configuration action for the disclosure</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown if participant ID not found</exception>
        public ActionBuilder Disclose(string participantId, Action<DisclosureBuilder> configure)
        {
            if (!_participantIds.Contains(participantId))
                throw new ArgumentException($"Participant '{participantId}' not found in blueprint");

            var disclosureBuilder = new DisclosureBuilder(participantId);
            configure(disclosureBuilder);
            _disclosures.Add(disclosureBuilder.Build());

            return this;
        }

        /// <summary>
        /// Specifies data fields to include in transaction metadata (unencrypted, for indexing)
        /// </summary>
        /// <param name="dataPointers">JSON Pointers to data fields</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder TrackData(params string[] dataPointers)
        {
            _disclosures.Add(new Disclosure(
                DisclosureConstants.TrackingData,
                dataPointers.ToList()));
            return this;
        }

        /// <summary>
        /// Makes specified data fields publicly accessible (unencrypted)
        /// </summary>
        /// <param name="dataPointers">JSON Pointers to data fields</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder MakePublic(params string[] dataPointers)
        {
            _disclosures.Add(new Disclosure(
                DisclosureConstants.PublicData,
                dataPointers.ToList()));
            return this;
        }

        /// <summary>
        /// Routes this action to a single next participant
        /// </summary>
        /// <param name="participantId">The participant ID to route to</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown if participant ID not found</exception>
        public ActionBuilder RouteToNext(string participantId)
        {
            if (!_participantIds.Contains(participantId))
                throw new ArgumentException($"Participant '{participantId}' not found in blueprint");

            _action.Participants = new List<Condition>
            {
                new Condition(participantId, true)
            };

            return this;
        }

        /// <summary>
        /// Defines conditional routing logic for this action
        /// </summary>
        /// <param name="configure">Configuration action for conditional routing</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder RouteConditionally(Action<ConditionBuilder> configure)
        {
            var conditionBuilder = new ConditionBuilder(_participantIds);
            configure(conditionBuilder);
            _action.Participants = conditionBuilder.BuildParticipantConditions();
            _action.Condition = conditionBuilder.BuildActionCondition();
            return this;
        }

        /// <summary>
        /// Specifies additional recipients for this action (similar to CC in email)
        /// </summary>
        /// <param name="participantIds">The participant IDs to also send to</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown if any participant ID not found</exception>
        public ActionBuilder AlsoSendTo(params string[] participantIds)
        {
            foreach (var id in participantIds)
            {
                if (!_participantIds.Contains(id))
                    throw new ArgumentException($"Participant '{id}' not found in blueprint");
            }

            _action.AdditionalRecipients = participantIds.ToList();
            return this;
        }

        /// <summary>
        /// Adds a calculation to this action using JSON Logic
        /// </summary>
        /// <param name="fieldName">The name of the calculated field</param>
        /// <param name="configure">Configuration action for the calculation</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder Calculate(string fieldName, Action<CalculationBuilder> configure)
        {
            var calcBuilder = new CalculationBuilder();
            configure(calcBuilder);
            _calculations[fieldName] = calcBuilder.Build();
            return this;
        }

        /// <summary>
        /// Defines the UI form for this action
        /// </summary>
        /// <param name="configure">Configuration action for the form</param>
        /// <returns>The builder instance for chaining</returns>
        public ActionBuilder WithForm(Action<FormBuilder> configure)
        {
            _formBuilder = new FormBuilder();
            configure(_formBuilder);
            return this;
        }

        internal Siccar.Application.Action Build()
        {
            // Build data schemas
            if (_schemaBuilder != null)
            {
                _action.DataSchemas = new List<JsonDocument> { _schemaBuilder.Build() };
            }

            // Set disclosures
            _action.Disclosures = _disclosures;

            // Set calculations
            _action.Calculations = _calculations;

            // Set form
            if (_formBuilder != null)
                _action.Form = _formBuilder.Build();

            return _action;
        }
    }
}
