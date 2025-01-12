using AutoMapper;
using backend.Config;
using backend.Dtos;
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
                    Password = "aB123123",
                },
                new()
                {
                    Email = "admin@gmail.com",
                    Role = UserRole.Admin,
                    Password = "aB123123",
                }
            ];

        private readonly LoginRequest _mockLoginRequest = new()
        {
            Email = "test@email.com",
            Role = "Customer",
            Password = "Pass123123"
        };

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
        public async Task CreateUser_ShouldCreateUser_WhenNoUserExists_WithSameEmail()
        {
            string newUserEmail = "newuser@example.com";
            await _sut.CreateUser(new UserCreationRequest()
            {
                Email = newUserEmail,
                Password = "Pass123123"
            });
            // Verifies that the Create method of the UserRepository was called with the correct email. Not checking password, since it will be hashed.
            _mockUserRepository.Verify(mock => mock.Create(It.Is<User>(p => p.Email == newUserEmail)), Times.Once());
        }

        [Theory]
        [InlineData("customeremail.com")]
        [InlineData("")]
        [InlineData("customer@email")]
        [InlineData("com.email@email")]
        public async Task CreateUser_ShouldThrowException_WhenInvalidEmail(string email)
        {
            var userCreationRequest = new UserCreationRequest()
            {
                Email = email,
                Password = "123123"
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _sut.CreateUser(userCreationRequest);
            });

            // Check the exception message and verify that Create() has not been called.
            Assert.Contains("Email is not valid", exception.Message);
            _mockUserRepository.Verify(mock => mock.Create(It.IsAny<User>()), Times.Never());
        }

        [Theory]
        [InlineData("")]
        [InlineData("aB123")]
        // Password is 31 characters
        [InlineData("aB12312312312312312312312312312")]
        public async Task CreateUser_ShouldThrowException_WhenPassWordHas_IncorrectLength(string password)
        {
            var userCreationRequest = new UserCreationRequest()
            {
                Email = "testcustomer@example.com",
                Password = password
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _sut.CreateUser(userCreationRequest);
            });

            Assert.Contains("6 and 30", exception.Message);
            _mockUserRepository.Verify(mock => mock.Create(It.IsAny<User>()), Times.Never());
        }

        [Theory]
        [InlineData("aB1234")]
        [InlineData("aB12345")]
        // Password with 29 characters
        [InlineData("aB123123123123123123123123123")]
        // Password with 30 characters
        [InlineData("aB1231231231231231231231231231")]
        public async Task CreateUser_ShouldCreateUser_WhenPassWordHas_CorrectLength(string password)
        {
            var userCreationRequest = new UserCreationRequest()
            {
                Email = "testcustomer@example.com",
                Password = password
            };

            await _sut.CreateUser(userCreationRequest);

            // Not using the password to check, since it has been hashed.
            _mockUserRepository.Verify(mock => mock.Create(It.Is<User>(p => p.Email == userCreationRequest.Email)), Times.Once());
        }

        [Theory]
        [InlineData("123123")]
        [InlineData("123123A")]
        [InlineData("123123a")]
        [InlineData("aBaBaB")]
        public async Task CreateUser_ShouldThrowException_WhenPassWordHas_IncorrectFormat(string password)
        {
            var userCreationRequest = new UserCreationRequest()
            {
                Email = "testcustomer@example.com",
                Password = password
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _sut.CreateUser(userCreationRequest);
            });

            Assert.Contains("must contain at least", exception.Message);
            _mockUserRepository.Verify(mock => mock.Create(It.IsAny<User>()), Times.Never());

        }

        [Theory]
        [InlineData("aB1231")]
        [InlineData("aBBBB1")]
        public async Task CreateUser_ShouldCreateUser_WhenPassWordHas_CorrectFormat(string password)
        {
            var userCreationRequest = new UserCreationRequest()
            {
                Email = "testcustomer@example.com",
                Password = password
            };

            await _sut.CreateUser(userCreationRequest);
            _mockUserRepository.Verify(mock => mock.Create(It.Is<User>(p => p.Email == userCreationRequest.Email)), Times.Once());
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

        [Fact]
        public void CheckPasswordValidation_ShouldReturnTrue_When_PasswordsMatch()
        {
            string matchingHashedPassword = "AQAAAAIAAYagAAAAELTgUXJVjB0nDV3ATcpryRfjQDbOgakXNXY9QDJvyDAgHLaKa0CPc7eFiB1WUr3lUg==";
            bool validPassword = _sut.CheckPasswordValidation(_mockLoginRequest.Password, matchingHashedPassword, _mockLoginRequest);
            Assert.True(validPassword);
        }

        [Fact]
        public void CheckPasswordValidation_ShouldReturnFalse_When_PasswordsDoNotMatch()
        {
            string hashedPasswordThatDoesNotMatch = "AQAAAAIAAYagAAAAEC6cop1QHZMe9j5x/K/OSHYPJssLCfc26kVDdigke13RVwqlNlgsGqbXKShhxdaScQ==";
            bool validPassword = _sut.CheckPasswordValidation(_mockLoginRequest.Password, hashedPasswordThatDoesNotMatch, _mockLoginRequest);
            Assert.False(validPassword);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnToken_When_ValidEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("JWTSecretKey", "123123123123123123123123123123123123123123123123");
            Environment.SetEnvironmentVariable("Issuer", "Issuer");
            Environment.SetEnvironmentVariable("Audience", "Audience");
            var token = _sut.GenerateJwtToken(_mockLoginRequest);
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_When_MissingEnvironmentVariables()
        // Made like this to accomodate the env-variables set in workflow.
        {
            var originalJwtSecretKey = Environment.GetEnvironmentVariable("JWTSecretKey");
            var originalIssuer = Environment.GetEnvironmentVariable("Issuer");
            var originalAudience = Environment.GetEnvironmentVariable("Audience");

            try
            {
                Environment.SetEnvironmentVariable("JWTSecretKey", null);
                Environment.SetEnvironmentVariable("Issuer", null);
                Environment.SetEnvironmentVariable("Audience", null);

                var exception = Assert.Throws<ArgumentNullException>(() => _sut.GenerateJwtToken(_mockLoginRequest));
            }
            finally
            {
                Environment.SetEnvironmentVariable("JWTSecretKey", originalJwtSecretKey);
                Environment.SetEnvironmentVariable("Issuer", originalIssuer);
                Environment.SetEnvironmentVariable("Audience", originalAudience);
            }
        }


    }
}
