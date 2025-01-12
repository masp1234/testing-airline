using AutoMapper;
using backend.Config;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace backend.Tests.Unit
{
    public class UserServiceUnitTests
    {
        private readonly IUserService _sut;

        private readonly List<User> _mockUsers =
            [
                new()
                {
                    Email = "testuser@gmail.com",
                    Role = UserRole.Customer,
                    Password = "123123",
                },
                new()
                {
                    Email = "admin@gmail.com",
                    Role = UserRole.Admin,
                    Password = "123123",
                }
            ];

        private readonly Mock<IUserRepository> _mockUserRepository;
   
        public UserServiceUnitTests() {
            _mockUserRepository = new Mock<IUserRepository>();

            // Not mocking PasswordHasher and MapperConfiguration, since they always do the same.
            var passwordHasher = new PasswordHasher<User>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });


            IMapper mapper = configuration.CreateMapper();

            _sut = new UserService(_mockUserRepository.Object, passwordHasher, mapper);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnUsers_WhenUsersExists()
        {
            _mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync(_mockUsers);
            var users = await _sut.GetUsers();
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            _mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync([]);
            var users = await _sut.GetUsers();
            Assert.Empty(users);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_When_UserExists()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(It.Is<string>(email => email == _mockUsers[0].Email))).ReturnsAsync(_mockUsers[0]);
            var user = await _sut.GetUserByEmail(_mockUsers[0].Email);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_When_UserDoesNotExist()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(It.Is<string>(email => email == _mockUsers[0].Email))).ReturnsAsync(_mockUsers[0]);
            _mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync([]);
            var users = await _sut.GetUsers();
            Assert.Empty(users);
        }


    }
}
