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

using Json.Schema.Generation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Siccar.Application
{
#nullable enable
    /// <summary>
    /// A Participant is defined by a Friendly Name and a Wallet Address
    /// </summary>
    public class Participant : IEquatable<Participant>
    {
        /// <summary>
        /// Transaction ID of a Published Participant
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// A Friendly Name For the Participant
        /// </summary>
        [DataMember]
        [Json.Schema.Generation.Required]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please enter {0}.")]
        public string Name { get; set; } = "";

        /// <summary>
        /// The Organisation the Participant belongs to
        /// </summary>
        [DataMember]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please enter {0}.")]
        public string Organisation { get; set; } = "";

        /// <summary>
        /// The Published Address of a Wallet
        /// </summary>
        [DataMember]
        [Json.Schema.Generation.Required]
        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please enter {0}.")]
        public string WalletAddress { get; set; } = "";

        /// <summary>
        /// The participant will use a devived/privacy address 
        /// Generally for an anonymous public user
        /// </summary>
        [DataMember]
        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please enter {0}.")]
        public string? didUri { get; set; } = "";

        /// <summary>
        /// The participant will use a devived/privacy address 
        /// Generally for an anonymous public user
        /// </summary>
        [DataMember]
        public bool useStealthAddress { get; set; } = false;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Participant);
        }

        public bool Equals(Participant? other)
        {
            return other != null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
#nullable restore
}

