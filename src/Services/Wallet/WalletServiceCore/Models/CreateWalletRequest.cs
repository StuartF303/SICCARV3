using System.ComponentModel.DataAnnotations;
#nullable enable

#nullable enable

namespace Siccar.Platform
{
    public class CreateWalletRequest
    {
        [Required]
        [StringLength(200)]
        public string? Name { get; set; }
        
        public string? Mnemonic { get; set; }
    }
}
