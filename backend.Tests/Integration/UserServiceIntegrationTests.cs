using AutoMapper;
using backend.Models;
using backend.Dtos;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using backend.Config;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Integration
{
    public class UserServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly UserService _sut;

        public UserServiceIntegrationTests(TestDatabaseFixture dbFixture) {
            _dbFixture = dbFixture;
            var passwordHasher = new PasswordHasher<User>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();
            _sut = new UserService(new UserRepository(_dbFixture.DbContext), passwordHasher, mapper);
      
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenNoUserExists_WithSameEmail()
        {
            await _sut.CreateUser(new UserCreationRequest()
            {
                Email = "customer@example.com",
                Password = "123123"
            });
        }

        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenUserWith_SameEmailExists()
        {
            // Act and Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _sut.CreateUser(new UserCreationRequest()
                {
                    Email = "customer@example.com",
                    Password = "123123"
                });
            });      
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_When_UserExists()
        {
           
            string userEmail = "customer@example.com";
            var user = await _sut.GetUserByEmail(userEmail);
            Assert.NotNull(user);
            Assert.True(user.Email == userEmail);

        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_When_UserDoesNotExists()
        {
            string nonexistingUserEmail = "test@example.com";
            var user = await _sut.GetUserByEmail(nonexistingUserEmail);
            Assert.Null(user);

        }
    }
}
