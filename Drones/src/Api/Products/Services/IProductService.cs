using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;

namespace Drones.src.Api.Products.Services
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request);
        Task<ProductResponse> GetAsync(Guid id);
        Task<List<ProductResponse>> GetAllAsync();
        Task<List<ProductResponse>> GetByCategoryAsync(int categoryId);
        Task<List<ProductResponse>> GetByRestaurantAsync(Guid restaurantId);
        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);
        Task DeleteAsync(Guid id);
    }
}
