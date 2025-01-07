﻿using AutoMapper;
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
        private readonly User _existingUser = new()
        {
            Email = "customer@example.com",
            Password = "123123"
        };

        private readonly LoginRequest loginRequest = new()
        {
            Email = "test@email.com",
            Role = "Customer",
            Password = "123123"
        };

        public UserServiceIntegrationTests(TestDatabaseFixture dbFixture) {
            _dbFixture = dbFixture;
            var passwordHasher = new PasswordHasher<User>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();
            _sut = new UserService(new UserRepository(_dbFixture.DbContext), passwordHasher, mapper);
            _dbFixture.ResetDatabase();

            // The double .Clear() of the ChangeTracker is so that every test gets a "fresh" context to work with.
            // If this is not done, there will be errors because of the tracked entities
            _dbFixture.DbContext.ChangeTracker.Clear();
            _dbFixture.DbContext.Users.Add(_existingUser);
            _dbFixture.DbContext.SaveChanges();
            _dbFixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnListOfUsers()
        {
            var users = await _sut.GetUsers();
            Assert.Single(users);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnEmptyList_When_NoUsersFound()
        {
            // Arrange
            _dbFixture.DbContext.Users.Remove(_existingUser);
            await _dbFixture.DbContext.SaveChangesAsync();

            // Act
            var users = await _sut.GetUsers();

            // Assert
            Assert.Empty(users);
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenNoUserExists_WithSameEmail()
        {
            string brandNewUserEmail = "newuser@example.com";
            await _sut.CreateUser(new UserCreationRequest()
            {
                Email = brandNewUserEmail,
                Password = "123123"
            });

            var createdUser = await _sut.GetUserByEmail(brandNewUserEmail);
            Assert.NotNull(createdUser);
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
            var user = await _sut.GetUserByEmail(_existingUser.Email);
            Assert.NotNull(user);
            Assert.True(user.Email == _existingUser.Email);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_When_UserDoesNotExists()
        {
            string nonexistingUserEmail = "test@example.com";
            var user = await _sut.GetUserByEmail(nonexistingUserEmail);
            Assert.Null(user);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Act
            var user = await _sut.GetUserById(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(_existingUser.Email, user.Email);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var user = await _sut.GetUserById(999);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task CheckUserByEmail_ShouldReturnJwtRequest_WhenUserExists()
        {
            var user = await _sut.CheckUserByEmail(_existingUser.Email);
            Assert.NotNull(user);
            Assert.True(user.Email == _existingUser.Email);
        }

        [Fact]
        public async Task CheckUserByEmail_ShouldReturnNull_When_UserDoesNotExists()
        {
            string nonexistingUserEmail = "test@example.com";
            var user = await _sut.CheckUserByEmail(nonexistingUserEmail);
            Assert.Null(user);
        }

        [Fact]
        public void CheckPasswordValidation_ShouldReturnTrue_When_PasswordsMatch()
        {
            string matchingHashedPassword = "AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==";
            bool validPassword = _sut.CheckPasswordValidation(loginRequest.Password, matchingHashedPassword, loginRequest);
            Assert.True(validPassword);
        }

        [Fact]
        public void CheckPasswordValidation_ShouldReturnFalse_When_PasswordsDoNotMatch()
        {
            string hashedPasswordThatDoesNotMatch = "AQAAAAIAAYagAAAAEC6cop1QHZMe9j5x/K/OSHYPJssLCfc26kVDdigke13RVwqlNlgsGqbXKShhxdaScQ==";

            bool validPassword = _sut.CheckPasswordValidation(loginRequest.Password, hashedPasswordThatDoesNotMatch, loginRequest);
            Assert.False(validPassword);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnToken_When_ValidEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("JWTSecretKey", "123123123123123123123123123123123123123123123123");
            Environment.SetEnvironmentVariable("Issuer", "Issuer");
            Environment.SetEnvironmentVariable("Audience", "Audience");
            var token = _sut.GenerateJwtToken(loginRequest);
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_When_MissingEnvironmentVariables()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GenerateJwtToken(loginRequest));
        }
    }
}
