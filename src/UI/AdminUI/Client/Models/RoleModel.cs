using System;

namespace Siccar.UI.Admin.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public bool Admin { get; set; }
        public bool Reader { get; set; }
        public bool Billing { get; set; }


    }
}
