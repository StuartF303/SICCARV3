namespace Siccar.EndToEndTests.Wallet.Models
{
    public class Wallet
    {
        public string? Address { get; init; }
        public string? Name { get; set; }
        public string? Owner { get; init; }
        public string? Tenant { get; init; }
        public virtual ICollection<WalletTransaction>? Transactions { get; set; }
    }
}
