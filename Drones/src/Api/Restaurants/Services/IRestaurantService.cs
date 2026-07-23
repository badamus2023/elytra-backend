using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;

namespace Drones.src.Api.Restaurants.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request);
        Task<RestaurantResponse> GetAsync(Guid id);
        Task<List<RestaurantResponse>> GetAllAsync();
        Task<RestaurantResponse> GetMineAsync(Guid userId);
        Task<RestaurantResponse> UpdateAsync(Guid id, UpdateRestaurantRequest request);
        Task<RestaurantResponse> UpdateOwnedAsync(Guid id, Guid userId, UpdateRestaurantRequest request);
        Task DeleteAsync(Guid id);
    }
}
