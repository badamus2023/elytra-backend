using Drones.src.Api.Restaurants.DTOs.Responses;

namespace Drones.src.Api.Restaurants.Services
{
    public interface IRestaurantApplicationService
    {
        Task<List<RestaurantApplicationResponse>> GetAllAsync();
        Task<RestaurantApplicationResponse> GetMineAsync(Guid userId);
        Task<RestaurantApplicationResponse> ApproveAsync(Guid id, string? note);
        Task<RestaurantApplicationResponse> RejectAsync(Guid id, string? note);
    }
}
