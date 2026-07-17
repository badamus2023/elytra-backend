namespace Drones.src.Api.Restaurants.DTOs.Requests
{
    public class CreateReviewRequest
    {
        public Guid RestaurantId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
