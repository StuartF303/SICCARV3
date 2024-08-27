using System.Collections.Generic;

namespace Siccar.UI.Admin.Models
{
    public class TxModel
    {
        public string TransactionId { get; set; }
        public string PrevTransactionId { get; set; }
        public string Submitted { get; set; }
        public string DocketId { get; set; }
        public string Validated { get; set; }
        public string SourceAddress { get; set; }
        public List<string> DestinationAddress { get; set; } = new List<string>();
        public string TransactionSize { get; set; }
        public string NumberOfPayloads { get; set; }
        public string MetaDataList { get; set; }
        public string PayloadList { get; set; }
        public string DestinationAddresses { get; set; }
    }
}
