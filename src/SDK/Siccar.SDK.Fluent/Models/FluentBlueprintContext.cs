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
