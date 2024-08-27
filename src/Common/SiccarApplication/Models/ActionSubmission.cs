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
using Json.Schema.Generation;
#nullable enable

namespace Siccar.Application
{
    /// <summary>
    /// The action submissions purpose is to submit a continuation 
    /// of data transport with in a flow this is the returned reponse 
    /// for the next Participant in the flow
    /// Validation is performed by Validation/ActionValidator
    /// </summary>

    public class ActionSubmission
    {

        /// <summary>
        /// The previous TxId that was used to generate this action
        /// </summary>
        [MaxLength(64)]
        [Required]
        public string PreviousTxId { get; set; } = string.Empty;

        /// <summary>
        /// Blueprint TX Id this action belongs to
        /// This maintains the 
        /// </summary>
        [MaxLength(64)]
        [Required]
        public string BlueprintId { get; set; } = string.Empty;

        /// <summary>
        /// Sending Wallet Address
        /// </summary>
        [Required]
        public string WalletAddress { get; set; } = string.Empty;

        /// <summary>
        /// The RegisterID of the Target Register
        /// </summary>
        [Required]
        public string RegisterId { get; set; } = string.Empty;

        /// <summary>
        /// The Submitted Json Data
        /// </summary>
        [Required]
        public JsonDocument? Data { get; set; }
 
    }
}
