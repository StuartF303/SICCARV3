#nullable enable

namespace Siccar.EndToEndTests.Blueprint.Models
{
    public class Blueprint
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Version { get; set; }
        public List<dynamic>? DataSchemas { get; set; }
        public List<dynamic>? Participants { get; set; }
        public List<Action>? Actions { get; set; }
    }
}
