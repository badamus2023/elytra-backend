namespace Drones.src.Api.Restaurants.DTOs.Responses
{
    public class RestaurantApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? RestaurantId { get; set; }
    }
}
