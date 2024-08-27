namespace Siccar.EndToEndTests.Blueprint.Models
{
    public class Transaction
    {
        public string? Id => TxId;
        public string? TxId { get; set; }
        public string? PrevTxId { get; set; }
        public int Version { get; set; }
        public string? SenderWallet { get; set; }
        public IEnumerable<string>? RecipientsWallets { get; set; }
        public DateTime TimeStamp { get; set; }
        public TransactionMetaData? MetaData { get; set; }
        public UInt64 PayloadCount { get; set; }
        public dynamic? Payloads { get; set; }
        public string? Signature { get; set; }
    }
}
