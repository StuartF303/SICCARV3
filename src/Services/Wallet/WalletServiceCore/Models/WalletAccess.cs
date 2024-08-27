using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
#nullable enable

namespace Siccar.Platform
{
    public class WalletAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }

        // Wallet to which the transaction belongs
        [MaxLength(64)]
        public string? WalletId { get; set; }

        public string? Tenant { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessTypes AccessType { get; set; } = AccessTypes.none;

        [Required]
        public string Reason { get; set; } = "";

        [Timestamp]
        [Required]
        public DateTime AssignedTime { get; set; } = DateTime.UtcNow;
    }

    public enum AccessTypes
    {
        none,
        owner,
        delegaterw,
        delegatero
    }

}
