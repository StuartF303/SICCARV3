namespace WalletService.IntegrationTests.Models
{
    public class WalletMessage
    {
        public string? Topic { get; set; }
        public WalletMessageData? Data { get; set; }
    }
}
