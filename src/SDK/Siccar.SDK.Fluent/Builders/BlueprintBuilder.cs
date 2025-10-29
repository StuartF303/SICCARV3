// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using FluentValidation;
using Siccar.Application;
using Siccar.Application.Validation;
using Siccar.SDK.Fluent.Models;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating SICCAR Blueprints with an intuitive, chainable API
    /// </summary>
    public class BlueprintBuilder
    {
        private readonly FluentBlueprintContext _context;
        private readonly List<Func<Blueprint, Blueprint>> _buildSteps;

        private BlueprintBuilder()
        {
            _context = new FluentBlueprintContext();
            _buildSteps = new List<Func<Blueprint, Blueprint>>();
        }

        /// <summary>
        /// Creates a new BlueprintBuilder instance
        /// </summary>
        /// <returns>A new BlueprintBuilder</returns>
        public static BlueprintBuilder Create() => new BlueprintBuilder();

        /// <summary>
        /// Sets the blueprint ID. If not called, a new GUID will be generated.
        /// </summary>
        /// <param name="id">The blueprint ID</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder WithId(string id)
        {
            _context.BlueprintId = id;
            _buildSteps.Add(bp => { bp.Id = id; return bp; });
            return this;
        }

        /// <summary>
        /// Sets the blueprint title
        /// </summary>
        /// <param name="title">The blueprint title (minimum 3 characters)</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder WithTitle(string title)
        {
            _buildSteps.Add(bp => { bp.Title = title; return bp; });
            return this;
        }

        /// <summary>
        /// Sets the blueprint description
        /// </summary>
        /// <param name="description">The blueprint description (minimum 5 characters)</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder WithDescription(string description)
        {
            _buildSteps.Add(bp => { bp.Description = description; return bp; });
            return this;
        }

        /// <summary>
        /// Sets the blueprint version
        /// </summary>
        /// <param name="version">The version number</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder WithVersion(int version)
        {
            _buildSteps.Add(bp => { bp.Version = version; return bp; });
            return this;
        }

        /// <summary>
        /// Adds a participant to the blueprint
        /// </summary>
        /// <param name="participantId">Unique identifier for the participant</param>
        /// <param name="configure">Configuration action for the participant</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder AddParticipant(string participantId, Action<ParticipantBuilder> configure)
        {
            var participantBuilder = new ParticipantBuilder(participantId);
            configure(participantBuilder);

            var participant = participantBuilder.Build();
            _context.Participants.Add(participantId, participant);

            _buildSteps.Add(bp =>
            {
                bp.Participants.Add(participant);
                return bp;
            });

            return this;
        }

        /// <summary>
        /// Adds an action to the blueprint
        /// </summary>
        /// <param name="actionId">Unique identifier for the action</param>
        /// <param name="configure">Configuration action for the action</param>
        /// <returns>The builder instance for chaining</returns>
        public BlueprintBuilder AddAction(string actionId, Action<ActionBuilder> configure)
        {
            var actionBuilder = new ActionBuilder(
                actionId,
                _context.Participants.Keys.ToList(),
                _context.Actions.Count + 1);

            configure(actionBuilder);

            var action = actionBuilder.Build();
            _context.Actions.Add(actionId, action);

            _buildSteps.Add(bp =>
            {
                bp.Actions.Add(action);
                return bp;
            });

            return this;
        }

        /// <summary>
        /// Builds the blueprint and validates it using the full validation ruleset
        /// </summary>
        /// <returns>The validated blueprint</returns>
        /// <exception cref="FluentValidationException">Thrown if validation fails</exception>
        public Blueprint Build()
        {
            var blueprint = new Blueprint
            {
                Id = _context.BlueprintId,
                Version = 1,
                Participants = new List<Participant>(),
                Actions = new List<Siccar.Application.Action>()
            };

            // Apply all build steps
            foreach (var step in _buildSteps)
            {
                blueprint = step(blueprint);
            }

            // Validate using the existing BlueprintValidator
            var validator = new BlueprintValidator();
            var validationResult = validator.Validate(blueprint, options =>
                options.IncludeRuleSets("Publish"));

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new FluentValidationException($"Blueprint validation failed: {errors}");
            }

            return blueprint;
        }

        /// <summary>
        /// Builds the blueprint without validation (useful for drafts)
        /// </summary>
        /// <returns>The blueprint without validation</returns>
        public Blueprint BuildDraft()
        {
            var blueprint = new Blueprint
            {
                Id = _context.BlueprintId,
                Version = 1,
                Participants = new List<Participant>(),
                Actions = new List<Siccar.Application.Action>()
            };

            foreach (var step in _buildSteps)
            {
                blueprint = step(blueprint);
            }

            return blueprint;
        }
    }
}
