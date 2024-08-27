namespace WalletService.IntegrationTests.Models
{
    public class CreateTransaction
    {
        public dynamic? TransactionMetaData { get; set; }
        public string? PreviousTransactionId { get; set; }
        public List<string>? ToWallets { get; set; }
        public List<dynamic>? PayloadsForWallets { get; set; }
    }
}
