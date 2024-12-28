using AutoMapper;
using backend.Database;
using backend.Models;
using backend.Models.MongoDB;
using backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.MongoDB
{
    public class UserMongoDBRepository(MongoDBContext context, IMapper mapper) : IUserRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task Create(User user)
        {
            var userMongo = _mapper.Map<UserMongo>(user);
            userMongo.Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks();
            await _context.Users.AddAsync(userMongo);
            await _context.SaveChangesAsync();

        }

        public async Task<List<User>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<User>>(users);
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return _mapper.Map<User?>(user);
        }

        public async Task<User?> GetUserById(long id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<User?>(user);
        }
    }
}
