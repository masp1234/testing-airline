using System.ComponentModel.DataAnnotations;

namespace backend.Dtos
{
    public class UserCreationRequest
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; }
    }
}
