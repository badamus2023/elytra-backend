using Drones.src.Api.Restaurants.Entities;

namespace Drones.src.Api.Auth.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public UserVerficationToken? VerificationToken { get; set; }
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
