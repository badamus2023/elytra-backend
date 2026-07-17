using Drones.src.Api.Restaurants.Entities;

namespace Drones.src.Api.Products.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public Restaurant Restaurant { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
