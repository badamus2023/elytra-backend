namespace Drones.src.Api.Products.DTOs.Requests
{
    public class CreateCategoryRequest
    {
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
