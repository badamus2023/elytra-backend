using Drones.src.Api.Auth.DTOs.Requests;
using Drones.src.Api.Auth.DTOs.Responses;

namespace Drones.src.Api.Auth.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RegisterRestaurantOwnerAsync(RegisterRestaurantOwnerRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task SendEmailVerificationAsync(string email);
        Task VerifyEmailAsync(string token);
        Task SendPasswordResetAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);
    }
}
