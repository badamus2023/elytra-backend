using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;

namespace Drones.src.Api.Products.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
        Task<CategoryResponse> GetAsync(int id);
        Task<List<CategoryResponse>> GetAllAsync();
        Task<List<CategoryResponse>> GetByRestaurantAsync(Guid restaurantId);
        Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);
        Task DeleteAsync(int id);
    }
}
