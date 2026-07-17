using Drones.src.Api.Data;
using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;
using Drones.src.Api.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Products.Services
{
    public class CategoryService: ICategoryService
    {

        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == request.RestaurantId)
                ?? throw new InvalidOperationException("RESTAURANT_NOT_FOUND");

            var category = new Category
            {
                RestaurantId = request.RestaurantId,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task<CategoryResponse> GetAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException("CATEGORY_NOT_FOUND");

            return MapToResponse(category);
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToResponse).ToList();
        }

        public async Task<List<CategoryResponse>> GetByRestaurantAsync(Guid restaurantId)
        {
            var categories = await _context.Categories
                .Where(c => c.RestaurantId == restaurantId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToResponse).ToList();
        }

        public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException("CATEGORY_NOT_FOUND");

            if (request.Name != null) category.Name = request.Name;
            if (request.Description != null) category.Description = request.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException("CATEGORY_NOT_FOUND");

            if (category.Products.Any())
                throw new InvalidOperationException("Cannot delete category with existing products.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        private static CategoryResponse MapToResponse(Category c) => new()
        {
            Id = c.Id,
            RestaurantId = c.RestaurantId,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        };
    }
}
