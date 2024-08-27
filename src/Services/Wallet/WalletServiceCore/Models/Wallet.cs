using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
#nullable enable

namespace Siccar.Platform
{
    public class Wallet : IEquatable<Wallet?>
    {
        /// <summary>
        /// The Primary Wallet address which acts as key for the wallet.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(64)]
        public string? Address { get; init; }

        /// <summary>
        /// The protected private key of the wallet, never exposed.
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public string? PrivateKey { get; init; }

        /// <summary>
        /// The wallets, human readable, name.
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(120), MinLength(1)]
        public string? Name { get; set; }

        /// <summary>
        /// The user who owns the wallet, not exposed in api responses.
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(40), MinLength(8)]
        public string? Owner { get; init; }

        /// <summary>
        /// The tenant to which the Wallet belongs
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(40), MinLength(8)]
        public string? Tenant { get; init; }


        /// <summary>
        /// List of delegates who can access this wallet
        /// </summary>
        public virtual ICollection<WalletAccess> Delegates { get; set; } = new List<WalletAccess>();

        /// <summary>
        /// List of alternate wallet addresses
        /// </summary>
        public virtual ICollection<WalletAddress>? Addresses { get; set; }

        /// <summary>
        /// List of recent transactions that are destined for this wallet
        /// </summary>
        public virtual ICollection<WalletTransaction>? Transactions { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Wallet);
        }

        public bool Equals(Wallet? other)
        {
            return other != null &&
                   Address == other.Address &&
                   PrivateKey == other.PrivateKey &&
                   Name == other.Name &&
                   Owner == other.Owner &&
                   Tenant == other.Tenant &&
                   EqualityComparer<ICollection<WalletAccess>>.Default.Equals(Delegates, other.Delegates) &&
                   EqualityComparer<ICollection<WalletAddress>>.Default.Equals(Addresses, other.Addresses) &&
                   EqualityComparer<ICollection<WalletTransaction>>.Default.Equals(Transactions, other.Transactions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Address, PrivateKey, Name, Owner, Tenant, Delegates, Addresses, Transactions);
        }
    }
}
