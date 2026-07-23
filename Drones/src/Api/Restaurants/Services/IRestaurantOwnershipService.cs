namespace Drones.src.Api.Restaurants.Services
{
    public interface IRestaurantOwnershipService
    {
        Task EnsureRestaurantAsync(Guid userId, Guid restaurantId);
        Task EnsureCategoryAsync(Guid userId, int categoryId);
        Task EnsureProductAsync(Guid userId, Guid productId);
    }
}
