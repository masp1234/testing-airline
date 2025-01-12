using System.ComponentModel.DataAnnotations;

namespace backend.Dtos
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public string? Role { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        public required string Password { get; set; }
    }
}