using Drones.src.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Restaurants.Services
{
    public class RestaurantOwnershipService : IRestaurantOwnershipService
    {
        private readonly AppDbContext _db;
        public RestaurantOwnershipService(AppDbContext db) => _db = db;
        public async Task EnsureRestaurantAsync(Guid userId, Guid restaurantId)
        {
            if (!await _db.Restaurants.AnyAsync(x => x.Id == restaurantId && x.OwnerUserId == userId))
                throw new UnauthorizedAccessException("You cannot manage this restaurant.");
        }
        public async Task EnsureCategoryAsync(Guid userId, int categoryId)
        {
            if (!await _db.Categories.AnyAsync(x => x.Id == categoryId && x.Restaurant.OwnerUserId == userId))
                throw new UnauthorizedAccessException("You cannot manage this category.");
        }
        public async Task EnsureProductAsync(Guid userId, Guid productId)
        {
            if (!await _db.Products.AnyAsync(x => x.Id == productId && x.Category.Restaurant.OwnerUserId == userId))
                throw new UnauthorizedAccessException("You cannot manage this product.");
        }
    }
}
