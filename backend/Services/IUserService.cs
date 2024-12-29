using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUsers();

        Task<User?> GetUserById(int id);

        Task<UserResponse?> GetUserByEmail(string email);
        Task<LoginRequest?> CheckUserByEmail(string email);

        Task CreateUser(UserCreationRequest userCreationRequest);

        string GenerateJwtToken(LoginRequest user);
        Boolean CheckPasswordValidation(string requestedPassword, string userPassword, LoginRequest user);
    }
}
