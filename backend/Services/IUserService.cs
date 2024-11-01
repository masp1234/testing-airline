﻿using backend.Dtos;

namespace backend.Services
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUsers();

        Task<UserResponse?> GetUserByEmail(string email);
        Task<JwtRequest?> CheckUserByEmail(string email);

        Task CreateUser(UserCreationRequest userCreationRequest);

        string GenerateJwtToken(JwtRequest user);
        Boolean CheckPasswordValidation(string requestedPassword, string userPassword, JwtRequest user);
    }
}
