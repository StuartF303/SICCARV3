using System.ComponentModel.DataAnnotations;

namespace Siccar.Platform.Tenants.Core
{
    public class Client : IdentityServer4.Models.Client
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string TenantId { get; set; } = string.Empty;
    }
}
