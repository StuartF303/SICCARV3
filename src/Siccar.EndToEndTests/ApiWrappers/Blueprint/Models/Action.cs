using System.Text.Json;
using System.Text.Json.Nodes;
using Siccar.Application;

namespace Siccar.EndToEndTests.Blueprint.Models
{
    public class Action
    {
        public int Id { get; set; } = 0;
        public string PreviousTxId { get; set; } = string.Empty;
        public string Blueprint { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public IEnumerable<dynamic>? Participants { get; set; }
        public IEnumerable<Disclosure>? Disclosures { get; set; } 
        public JsonDocument? Received { get; set; }
        public JsonDocument? Requested { get; set; }
        public IEnumerable<dynamic>? DataSchemas { get; set; }
        public dynamic? Condition { get; set; }
        public Control? Form { get; set; }
    }
}
