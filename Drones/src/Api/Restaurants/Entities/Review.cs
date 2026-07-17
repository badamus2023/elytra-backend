using Drones.src.Api.Auth.Entities;

namespace Drones.src.Api.Restaurants.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RestaurantId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}

        public User User { get; set; } = default!;
        public Restaurant Restaurant { get; set; } = default!;
    }
}
