using backend.Dtos;

namespace backend.Services
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUsers();

        Task<UserResponse?> GetUserByEmail(string email);

        Task CreateUser(UserCreationRequest userCreationRequest);
    }
}
