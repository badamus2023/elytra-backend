namespace Drones.src.Api.Restaurants.DTOs.Requests
{
    public class UpdateReviewRequest
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
