using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Siccar.Platform
{
    /// <summary>
    /// A Wallet Address used for messaging registration of an new System Address 
    /// </summary>
    public class WalletAddress
    {
        /// <summary>
        /// The Wallet ID is the Primary public address with no derivation 
        /// </summary>
        [MaxLength(64)]
        public string WalletId { get; set; } = "";

        /// <summary>
        /// A Derived Address that is subordinate to the Wallet public Key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(64)]
        public string Address { get; set; } = "";

        /// <summary>
        /// A Derived Address that is subordinate to the Wallet public Key
        /// </summary>
        [MinLength(2), MaxLength(255)]
        public string DerivationPath { get; set; } = "m/";
    }
}
