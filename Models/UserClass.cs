using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public class UserClass
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public decimal Balance { get; set; }
    }
} 