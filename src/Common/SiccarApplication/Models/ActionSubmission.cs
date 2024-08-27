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
