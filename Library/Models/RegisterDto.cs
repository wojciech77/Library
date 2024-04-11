using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public int RoleId { get; set; } = 1;
    }
}
