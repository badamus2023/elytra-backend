namespace Drones.src.Api.Products.DTOs.Requests
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public double? WeightKg { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
