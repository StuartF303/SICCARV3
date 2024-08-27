using Json.More;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
#nullable enable

namespace Siccar.Platform
{
    /// <summary>
    /// A Sealed Collection of Agreed Transactions
    /// </summary>
    public class Docket
    {
        /// <summary>
        /// This is likely a POSITIVE INTEGER as effectivly the block number
        /// </summary>
        [Key]
        [Required]
        public ulong Id { get; set; } = 0L;

        /// <summary>
        /// This is the Register ID
        /// </summary>
        [ForeignKey("Register")]
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "{0} has a min length of {2} and max length of {1} ")]
        [RegularExpression("^[a-z]+", ErrorMessage = "lowercase a-z only")]
        public string RegisterId { get; set; } = "";

        /// <summary>
        /// The Hash of the previous block to maintain the chain of custody
        /// </summary>
        [Required]
        public string PreviousHash { get; set; } = "";

        /// <summary>
        /// The Hash of the current block to maintain the chain of custody
        /// </summary>
        [Required]
        public string Hash { get; set; } = "";

        /// <summary>
        /// Not sure why this is here!!
        /// </summary>
        public string Votes { get; set; } = "";

        /// <summary>
        /// We are a special transaction type
        /// </summary>
        public TransactionMetaData MetaData { get; set; } = new TransactionMetaData() {
             TransactionType = TransactionTypes.Docket
        };
                        
        /// <summary>
        /// A list of Transactions sealed by this Docket
        /// </summary>
        public List<string> TransactionIds { get; set; } = new List<string>();

        /// <summary>
        /// A navigation to Transactions sealed by this Docket
        /// </summary>
        [JsonIgnore]
        public virtual IEnumerable<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

        /// <summary>
        /// The Docket TimeStamp, always in UTC
        /// </summary>
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// The Current State of the Docket
        /// </summary>
        [Required]
        public DocketState State { get; set; } = DocketState.Init;

    }

    /// <summary>
    /// Potentiual States of a Docket
    /// </summary>
    [JsonConverter(typeof(EnumStringConverter<DocketState>))]
    public enum DocketState
    {
        /// <summary>
        /// Initial State
        /// </summary>
        Init,
        /// <summary>
        /// The docket has been presented to the network for acceptance
        /// </summary>
        Proposed,
        /// <summary>
        /// The Network has ACCEPTED the Docket to be sealed
        /// </summary>
        Accepted,
        /// <summary>
        /// The Network has REJECT the Docket
        /// </summary>
        Rejected,
        /// <summary>
        /// The Docket has been SEALED
        /// </summary>
        Sealed
    }
}