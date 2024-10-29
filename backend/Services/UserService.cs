using AutoMapper;
using backend.Dtos;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            List<User> users = await _userRepository.GetAll();
            var mappedUsers = _mapper.Map<List<UserResponse>>(users);
            return mappedUsers;
        }

        public async Task<UserResponse?> GetUserByEmail(string email)
        {
            User? user = await _userRepository.GetByEmail(email);
            var mappedUser = _mapper.Map<UserResponse>(user);
            return mappedUser;
        }

        public async Task CreateUser(UserCreationRequest userCreationRequest)
        {
            User userToCreate = new User { 
                Email = userCreationRequest.Email,
                Role = UserRole.Customer
                                            
            };

            string hashedPassword = _passwordHasher.HashPassword(userToCreate, userCreationRequest.Email);
            userToCreate.Password = hashedPassword;
            await _userRepository.Create(userToCreate);
        }
    }
}
