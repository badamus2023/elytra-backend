namespace Drones.src.Api.Auth.DTOs.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public UserDto User { get; set; } = default;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } =[];
    }
}
