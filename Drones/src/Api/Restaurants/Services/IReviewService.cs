using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;

namespace Drones.src.Api.Restaurants.Services
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateAsync(Guid userId, CreateReviewRequest request);
        Task<List<ReviewResponse>> GetByRestaurantAsync(Guid restaurantId);
        Task<ReviewResponse> UpdateAsync(Guid reviewId, Guid userId, int rating, string? comment);
        Task DeleteAsync(Guid reviewId, Guid userId, bool isAdmin = false);
    }
}
