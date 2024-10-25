using backend.Database;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User> Add(User user);
    }
}
