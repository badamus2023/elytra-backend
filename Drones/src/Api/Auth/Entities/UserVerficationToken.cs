namespace Drones.src.Api.Auth.Entities
{
    public class UserVerficationToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }

        public User User { get; set; }
    }
}
