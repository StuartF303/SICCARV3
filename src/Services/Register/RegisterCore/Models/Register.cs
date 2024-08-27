using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Json.More;

namespace Siccar.Platform
{
    /// <summary>
    /// The Definition of a Data Register
    /// </summary>
    public class Register
    {
        
        /// <summary>
        /// A Unique Register ID
        /// </summary>
        [DataMember]
        [Key]
        [Display(Name = "Register ID")]
        [RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{12}(\}){0,1}$", ErrorMessage = "Must be a guid without hyphens")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// A Firendly Name for the Register
        /// </summary>
        [DataMember]
        [Required]
        [Display(Name = "Register Name")]
        [MaxLength(38, ErrorMessage = "Max 38 characters")]
        public string Name { get; set; } = "";

        /// <summary>
        /// A view of the processing height of the register
        /// </summary>
        public uint Height { get; set; } = 0;

        /// <summary>
        /// Not quite shure why this is in here at the moment
        /// </summary>
        public string Votes { get; set; } = "";

        /// <summary>
        /// A vitrual link to an enumeration of Sealed Transactions
        /// </summary>
        [JsonIgnore]
        [ForeignKey("Docket")]
        public virtual IEnumerable<Docket> Dockets { get; set; } = new List<Docket>();

        /// <summary>
        /// A virtual link to an enumeration of the transactions themselves
        /// </summary>
        [JsonIgnore]
        [ForeignKey("Transaction")]
        public virtual IEnumerable<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

        /// <summary>
        /// Is this Register publicly advertised on the Network 
        /// </summary>
        public bool Advertise { get; set; } = false;

        /// <summary>
        /// Does the local installation store all transactions for the register
        /// </summary>
        public bool IsFullReplica { get; set; } = true;

        /// <summary>
        /// What state is our local register in
        /// </summary>
        public RegisterStatusTypes Status { get; set; } = RegisterStatusTypes.OFFLINE;

    }

    /// <summary>
    /// Potential states for a register
    /// </summary>
    [JsonConverter(typeof(EnumStringConverter<RegisterStatusTypes>))]
    public enum RegisterStatusTypes
    {
        /// <summary>
        /// Out of Sync and not working
        /// </summary>
        OFFLINE = -1,
        /// <summary>
        /// Online, in sync and active
        /// </summary>
        ONLINE = 0,
        /// <summary>
        /// Online and validating 
        /// </summary>
        CHECKING = 1,
        /// <summary>
        /// Online and in the 
        /// </summary>
        RECOVERY = 2
    }
}
