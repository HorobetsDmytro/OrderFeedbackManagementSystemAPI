using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<User> GetCurrentUserAsync();
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(3)]
        public string Username { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
