// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Application;

namespace Siccar.SDK.Fluent.Models
{
    /// <summary>
    /// Internal state management for the fluent blueprint builder
    /// </summary>
    internal class FluentBlueprintContext
    {
        /// <summary>
        /// Dictionary of participants by their ID
        /// </summary>
        public Dictionary<string, Participant> Participants { get; }

        /// <summary>
        /// Dictionary of actions by their ID
        /// </summary>
        public Dictionary<string, Siccar.Application.Action> Actions { get; }

        /// <summary>
        /// The blueprint ID
        /// </summary>
        public string BlueprintId { get; set; }

        public FluentBlueprintContext()
        {
            Participants = new Dictionary<string, Participant>();
            Actions = new Dictionary<string, Siccar.Application.Action>();
            BlueprintId = Guid.NewGuid().ToString();
        }
    }
}
