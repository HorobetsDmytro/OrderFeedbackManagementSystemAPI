using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;

namespace OrderFeedbackManagementSystemAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already registered");

            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername != null)
                throw new InvalidOperationException("Username already taken");

            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                Password = HashPassword(request.Password),
                Role = "User"
            };

            await _userRepository.AddAsync(user);

            return new AuthResponse
            {
                Id = user.Id,
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!VerifyPassword(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            return new AuthResponse
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User is not authenticated");

            var userId = int.Parse(userIdClaim);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            return user;
        }
    }
}