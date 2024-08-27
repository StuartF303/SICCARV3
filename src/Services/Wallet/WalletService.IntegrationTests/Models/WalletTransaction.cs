namespace WalletService.IntegrationTests.Models
{
    public class WalletTransaction
    {
        public string? Id { get; set; }
        public string? TransactionId { get; set; }
        public string? WalletId { get; set; }
        public string? ReceivedAddress { get; set; }
        public string? PreviousId { get; set; }
        public string? Sender { get; set; }
        public bool isSendingWallet { get; set; }
        public bool isConfirmed { get; set; }
        public bool isSpent { get; set; } = false;
        public int MetaDataId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
