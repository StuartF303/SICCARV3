#nullable enable

namespace Siccar.Platform
{
    public class UpdateWalletRequest
    {
        public string? Name { get; set; }
        public List<WalletAddress>? Addresses { get; set; }
        public List<PendingTransaction>? PendingTransactions { get; set; }
    }
}
