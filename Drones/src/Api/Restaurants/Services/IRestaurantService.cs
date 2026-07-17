using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;

namespace Drones.src.Api.Restaurants.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request);
        Task<RestaurantResponse> GetAsync(Guid id);
        Task<List<RestaurantResponse>> GetAllAsync();
        Task<RestaurantResponse> UpdateAsync(Guid id, UpdateRestaurantRequest request);
        Task DeleteAsync(Guid id);
    }
}
