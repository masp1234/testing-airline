using backend.Models;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User?> GetUserById(int id);

        Task<User?> GetByEmail(string email);

        Task Create(User user);
    }
}
