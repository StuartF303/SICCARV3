namespace WalletService.IntegrationTests.Models
{
    public class UpdateWallet
    {
        public string? Name { get; set; }
        public List<dynamic>? Addresses { get; set; }
        public List<DuplicateWaitObjectException>? PendingTransactions { get; set; }
    }
}
