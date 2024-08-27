using Siccar.Platform;

namespace Siccar.EndToEndTests.Wallet.Models
{
    public class CreateWalletResponse
    {
        public string? Address { get; init; }
        public string? PrivateKey { get; init; }
        public string? Name { get; set; }
        public string? Owner { get; init; }
        public string? Tenant { get; init; }
        public virtual ICollection<WalletAccess> Delegates { get; set; } = new List<WalletAccess>();
        public virtual ICollection<WalletAddress>? Addresses { get; set; }
        public virtual ICollection<WalletTransaction>? Transactions { get; set; }
        public string? Mnemonic { get; init; }
    }
}
