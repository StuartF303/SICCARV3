using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
#nullable enable

namespace Siccar.Platform
{
    /// <summary>
    /// Data Definition for a Transaction Received or Transmitted via a Wallet
    /// </summary>
    public class WalletTransaction
    {
        // Transaction Id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(128)]
        public string Id { get { return $"{TransactionId}:{WalletId}"; } set { } }

        [MaxLength(64)]
        public string? TransactionId { get; set; }

        // Wallet to which the transaction belongs
        [MaxLength(64)]
        public string? WalletId { get; set; }

        [DataMember]
        [MaxLength(64)]
        public string? ReceivedAddress { get; set; }

        [DataMember]
        public string? PreviousId { get; set; }

        [DataMember]
        [MaxLength(64)]
        public string? Sender { get; set; }

        [DataMember]
        public bool isSendingWallet { get { return WalletId == Sender; } set { } }  // False means you are not the sender

        [DataMember]
        public bool isConfirmed { get; set; } = false;  // False means the transaction has not been Validated

        [DataMember]
        public bool isSpent { get; set; } = false;  // False means the transaction has not been responded to

        [DataMember]
        public TransactionMetaData? MetaData { get; set; }

        public int MetaDataId { get; set; }

        [Timestamp]
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
