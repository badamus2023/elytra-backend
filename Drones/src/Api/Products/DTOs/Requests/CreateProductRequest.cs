namespace Drones.src.Api.Products.DTOs.Requests
{
    public class CreateProductRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public double WeightKg { get; set; }
    }
}
