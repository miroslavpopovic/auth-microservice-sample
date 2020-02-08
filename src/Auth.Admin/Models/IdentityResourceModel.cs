using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auth.Admin.Models
{
    public class IdentityResourceModel
    {
        public IdentityResourceModel()
        {
            UserClaims = new List<string>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; } = true;

        public bool ShowInDiscoveryDocument { get; set; } = true;

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public List<string> UserClaims { get; set; }

        public string UserClaimsItems { get; set; }
    }
}
