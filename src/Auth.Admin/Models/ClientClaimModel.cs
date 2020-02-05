using System.ComponentModel.DataAnnotations;

namespace Auth.Admin.Models
{
    public class ClientClaimModel
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
