using Drones.src.Api.Auth.DTOs.Requests;
using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.Auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("register-restaurant-owner")]
        public async Task<ActionResult<AuthResponse>> RegisterRestaurantOwner(
            RegisterRestaurantOwnerRequest request)
        {
            return Ok(await _authService.RegisterRestaurantOwnerAsync(request));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<ActionResult<MessageResponse>> Revoke(RefreshTokenRequest request)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return Ok(new { message = "Token revoked." });
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<MessageResponse>> VerifyEmail(VerifyEmailRequest request)
        {
            await _authService.VerifyEmailAsync(request.Token);
            return Ok(new { message = "Email verified." });
        }

        [HttpPost("resend-verification")]
        public async Task<ActionResult<MessageResponse>> ResendVerification(ForgotPasswordRequest request)
        {
            await _authService.SendEmailVerificationAsync(request.Email);
            return Ok(new { message = "Verification email sent." });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<MessageResponse>> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.SendPasswordResetAsync(request.Email);
            return Ok(new { message = "If that email exists you will receive a reset link." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<MessageResponse>> ResetPassword(ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            return Ok(new { message = "Password reset successfully." });
        }
    }
}
