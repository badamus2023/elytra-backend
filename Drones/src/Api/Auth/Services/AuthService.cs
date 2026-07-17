using Drones.src.Api.Auth.DTOs.Requests;
using Drones.src.Api.Auth.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Net.WebSockets;
using Drones.src.Api.Auth.Entities;
using Drones.src.Api.Data;

namespace Drones.src.Api.Auth.Services
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext context, ITokenService tokenService, IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);

            if (existingUser)
            {
                throw new InvalidOperationException("EMAIL_TAKEN");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                IsEmailVerified = false,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
            };

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User") 
                ?? throw new InvalidOperationException("DEFAULT_ROLE_NOT_FOUND");

            user.UserRoles = new List<UserRole>
            {
                new UserRole { UserId = user.Id, RoleId = userRole.Id, AssignedAt = DateTime.UtcNow }
            };

            var verificationToken = new UserVerficationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _context.Users.AddAsync(user);
            await _context.UserVerficationTokens.AddAsync(verificationToken);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, verificationToken.Token);

            var roles = new List<string> { userRole.Name };
            var accessToken = _tokenService.GenerateAccessToken(user,roles);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return BuildAuthResponse(user, accessToken, refreshToken.Token, roles);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email)
                ?? throw new InvalidOperationException("INVALID_CREDENTIALS");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new InvalidOperationException("INVALID_CREDENTIALS");

            if(!user.IsEmailVerified)
                throw new InvalidOperationException("EMAIL_NOT_VERIFIED");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("ACCOUNT_DISABLED");

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return BuildAuthResponse(user, accessToken, refreshToken.Token, roles);
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);

            var storedToken = await _context.RefreshTokens.Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(rt => rt.Token == tokenHash) 
                        ?? throw new InvalidOperationException("INVALID_REFRESH_TOKEN");

            if (storedToken.RevokedAt != null)
            {
                throw new UnauthorizedAccessException("TOKEN_REVOKED");
            }

            if(storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("TOKEN_EXPIRED");
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            var user = storedToken.User;
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = await CreateRefreshTokenAsync(user.Id);

            storedToken.ReplacedByToken = newRefreshToken.Token;

            await _context.SaveChangesAsync();

            return BuildAuthResponse(user, newAccessToken, newRefreshToken.Token, roles);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);

            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenHash)
                ?? throw new InvalidOperationException("INVALID_REFRESH_TOKEN");

            storedToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task VerifyEmailAsync(string token)
        {
            var verficationToken = await _context.UserVerficationTokens
                .Include(vt => vt.User)
                .FirstOrDefaultAsync(vt => vt.Token == token)
                ?? throw new InvalidOperationException("INVALID_TOKEN");

            if (verficationToken.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("TOKEN_EXPIRED");

            verficationToken.User.IsEmailVerified = true;
            verficationToken.User.IsActive = true;

            _context.UserVerficationTokens.Remove(verficationToken);
            await _context.SaveChangesAsync();
        }

        public async Task SendEmailVerificationAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new InvalidOperationException("USER_NOT_FOUND");

            if (user.IsEmailVerified)
                throw new InvalidOperationException("EMAIL_ALREADY_VERIFIED");

            var token = new UserVerficationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _context.UserVerficationTokens.AddAsync(token);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, token.Token);
        }

        public async Task SendPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return;

            var rawToken = Guid.NewGuid().ToString("N");

            var token = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = HashToken(rawToken),
                ExpiresAt= DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };

            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();

            await _emailService.SendPasswordResetEmailAsync(user.Email, rawToken);
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var tokenHash = HashToken(token);

            var resetToken = await _context.PasswordResetTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == tokenHash && !rt.IsUsed)
                ?? throw new InvalidOperationException("INVALID_TOKEN");

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("TOKEN_EXPIRED");

            resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();
        }

        //Helpers

        private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
        {
            var rawToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = HashToken(rawToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            refreshToken.Token = rawToken;
            return refreshToken;
        }

        private static string HashToken(string token)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));

            return Convert.ToHexString(bytes).ToLower();
        }

        private static AuthResponse BuildAuthResponse(User user, string accessToken, string refreshToken, List<string> roles)
        {
            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles
                }
            };
        }
    }
}
