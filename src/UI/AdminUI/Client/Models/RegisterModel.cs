namespace Siccar.UI.Admin.Models
{
    public class RegisterModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public uint Height { get; set; }
        public string Votes { get; set; }
        public bool Advertise { get; set; }
        public bool IsFullReplica { get; set; }
        public Platform.RegisterStatusTypes Status { get; set; }
    }
}
