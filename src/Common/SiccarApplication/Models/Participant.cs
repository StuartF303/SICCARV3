// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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

