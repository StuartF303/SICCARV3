using System.Collections.Generic;
using System.Text.Json.Serialization;
#nullable enable

namespace Siccar.UI.Admin.Models
{
    public class DataSchema
    {
        [JsonPropertyName("$schema")]
        public string? Schema { get; set; } = "http://json-schema.org/draft-07/schema";
        [JsonPropertyName("$id")]
        public string? Id { get; set; } = "https://siccar.net/";
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "object";
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new List<string>();
        [JsonPropertyName("properties")]
        public Dictionary<string, DataSchemaProperty> Properties { get; set; } = new Dictionary<string, DataSchemaProperty>();
    }
}
