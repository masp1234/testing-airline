using backend.Database;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User?> GetByEmail(string email);

        Task Create(User user);
    }
}
