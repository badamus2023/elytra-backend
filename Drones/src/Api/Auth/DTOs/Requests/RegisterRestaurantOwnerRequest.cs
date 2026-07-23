namespace Drones.src.Api.Auth.DTOs.Requests
{
    public class RegisterRestaurantOwnerRequest : RegisterRequest
    {
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
    }
}
