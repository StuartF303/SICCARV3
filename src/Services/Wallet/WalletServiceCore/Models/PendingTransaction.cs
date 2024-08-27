using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable enable

namespace Siccar.Platform
{
    public class PendingTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string SenderWallet { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string TxId { get; set; } = "";

        [Required]
        [NotMapped]
        public TransactionMetaData? MetaData { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is PendingTransaction transaction &&
                   Id == transaction.Id &&
                   EqualityComparer<TransactionMetaData>.Default.Equals(MetaData, transaction.MetaData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MetaData);
        }
    }
}
