using System;

namespace Siccar.UI.Admin.Models
{
    public class RegisterDetailRawTransaction
    {
        public string TxId { get; set; } = "";
        public string SenderWallet { get; set; } = "";
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public bool IsFirstRow { get; set; }
    }
}
