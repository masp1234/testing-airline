namespace backend.Dtos
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public string? Role { get; set; }
        public required string Password { get; set; }
    }
}