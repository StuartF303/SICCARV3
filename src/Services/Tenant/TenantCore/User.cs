namespace Siccar.Platform
{
    public class User
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Tenant { get; set; }
        public List<string>? Roles { get; set; }
    }
}
