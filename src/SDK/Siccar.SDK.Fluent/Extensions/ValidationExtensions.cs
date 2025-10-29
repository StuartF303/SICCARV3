// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Application;
using Siccar.SDK.Fluent.Builders;
using Siccar.SDK.Fluent.Models;

namespace Siccar.SDK.Fluent.Extensions
{
    /// <summary>
    /// Extension methods for validation in the fluent SDK
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates the blueprint inline during construction
        /// </summary>
        /// <param name="builder">The blueprint builder</param>
        /// <param name="validator">Custom validation logic</param>
        /// <returns>The builder instance for chaining</returns>
        public static BlueprintBuilder ValidateInline(
            this BlueprintBuilder builder,
            Action<Blueprint> validator)
        {
            var draft = builder.BuildDraft();
            validator(draft);
            return builder;
        }

        /// <summary>
        /// Ensures a participant exists in the blueprint
        /// </summary>
        /// <param name="blueprint">The blueprint</param>
        /// <param name="participantId">The participant ID to check</param>
        /// <exception cref="FluentValidationException">Thrown if participant not found</exception>
        public static void EnsureParticipantExists(
            this Blueprint blueprint,
            string participantId)
        {
            if (!blueprint.Participants.Any(p => p.Id == participantId))
                throw new FluentValidationException($"Participant '{participantId}' not found in blueprint");
        }

        /// <summary>
        /// Ensures an action exists in the blueprint
        /// </summary>
        /// <param name="blueprint">The blueprint</param>
        /// <param name="actionId">The action ID to check</param>
        /// <exception cref="FluentValidationException">Thrown if action not found</exception>
        public static void EnsureActionExists(
            this Blueprint blueprint,
            int actionId)
        {
            if (!blueprint.Actions.Any(a => a.Id == actionId))
                throw new FluentValidationException($"Action with ID {actionId} not found in blueprint");
        }
    }
}
