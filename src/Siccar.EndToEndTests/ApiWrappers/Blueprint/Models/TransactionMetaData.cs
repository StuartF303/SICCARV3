namespace Siccar.EndToEndTests.Blueprint.Models
{
    public class TransactionMetaData
    {
        public int Id { get; set; }
        public string? RegisterId { get; set; }
        public string? TransactionType { get; set; }
        public string? BlueprintId { get; set; }
        public string? InstanceId { get; set; }
        public int ActionId { get; set; }
        public int NextActionId { get; set; }
    }
}
