using AutoMapper;
using backend.Models;
﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Dtos;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<User?> GetUserById(long id)
        {
            User? user = await _userRepository.GetUserById(id);
            return user;
        }

        public async Task<UserResponse?> GetUserByEmail(string email)
        {
            User? user = await _userRepository.GetByEmail(email);
            var mappedUser = _mapper.Map<UserResponse>(user);
            return mappedUser;
        }
        public async Task<JwtRequest?> CheckUserByEmail(string email)
        {
            User? user = await _userRepository.GetByEmail(email);
            var mappedUser = _mapper.Map<JwtRequest>(user);
            return mappedUser;
        }
        public Boolean CheckPasswordValidation(string requestedPassword, string userPassword, JwtRequest user)
        {
            var passwordHasher = new PasswordHasher<JwtRequest>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, userPassword, requestedPassword);
             if (verificationResult == PasswordVerificationResult.Success){
                return true;
             }
            return false;
        }

        public async Task CreateUser(UserCreationRequest userCreationRequest)
        {
            User userToCreate = new User { 
                Email = userCreationRequest.Email,
                Role = UserRole.Customer
                                            
            };

            string hashedPassword = _passwordHasher.HashPassword(userToCreate, userCreationRequest.Password);
            userToCreate.Password = hashedPassword;
            await _userRepository.Create(userToCreate);
        }

        public string GenerateJwtToken(JwtRequest user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSecretKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("Issuer"),
                audience: Environment.GetEnvironmentVariable("Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
