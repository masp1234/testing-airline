namespace backend.Dtos
{
    public class UserCreationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string RepeatedPassword { get; set; }
    }
}
