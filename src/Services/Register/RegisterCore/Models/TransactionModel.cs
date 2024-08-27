using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
#nullable enable

namespace Siccar.Platform
{
    /// <summary>
    /// A Basic Transaction within the syste,
    /// </summary>
    public class TransactionModel
    {
        /// <summary>
        /// The Transaction Hash is its ID
        /// </summary>
        [DataMember]
        [Required]
        [Key]
        [BsonId]
        [StringLength(64, ErrorMessage = "{0} are 64 Chars")]
        [RegularExpression("^[a-fA-F0-9]{64}$", ErrorMessage = "Hex Values Only")]
        public string Id => TxId;
        //TODO: Remove TxId and update all services to use Id
        public string TxId { get; set; } = String.Empty;


        /// <summary>
        /// The previous Transaction Hash ID
        /// </summary>
        [DataMember]
        [StringLength(64, ErrorMessage = "{0} are 64 Chars")]
        [RegularExpression("^[a-fA-F0-9]{64}$", ErrorMessage = "Hex Values Only")]
        public string PrevTxId {  get; set; } = String.Empty;

        /// <summary>
        /// The type of Transaction in the Register
        /// </summary>
        [DataMember]
        public UInt32 Version { get; set; }

        /// <summary>
        /// The Sender Address, 
        /// </summary>
        /// <remarks>
        /// In Base58 so limited characters
        /// </remarks>
        /// <example> 
        /// 'example address'
        /// </example>
        [Required]
        [DataMember]
        // temp disabled : [RegularExpression("^[a-km-zA-HJ-NP-Z1-9]{26,35}$", ErrorMessage = "Not a valid Address")]
        public string SenderWallet { get; set; } = "";

        /// <summary>
        /// The Recipient Address
        /// </summary>
        [Required]
        [DataMember]
        public IEnumerable<string> RecipientsWallets { get; set; } = new List<string>();

        /// <summary>
        /// UTC of when the Transaction was created
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The metadata of the transaction
        /// </summary>
        [Required]
        [DataMember]
        public TransactionMetaData? MetaData { get; set; } = new TransactionMetaData();

        /// <summary>
        /// The Transaction payload data - can we calclaute this....
        /// </summary>
        [DataMember]
        public UInt64 PayloadCount { get; set; } = 0L;

        /// <summary>
        /// The Transaction encoded payload data
        /// </summary>
        [Required]
        [DataMember]
        public PayloadModel[] Payloads { get; set; } = Array.Empty<PayloadModel>();

        /// <summary>
        /// The Transaction signature
        /// </summary>
        [Required]
        [DataMember]
        public string Signature { get; set; } = String.Empty;

    }
}
