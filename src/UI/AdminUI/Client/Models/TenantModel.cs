namespace Siccar.UI.Admin.Models
{
    public class TenantModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Authority { get; set; }
        public int Admins { get; set; }
        public int Clients { get; set; }
        public int Registers { get; set; }
        public int AccountsCount { get; set; }

    }
}
