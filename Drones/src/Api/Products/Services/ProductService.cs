using Drones.src.Api.Data;
using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;
using Drones.src.Api.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId)
                ?? throw new InvalidOperationException("CATEGORY_NOT_FOUND");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                WeightKg = request.WeightKg,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return MapToResponse(product, category.Name);
        }

        public async Task<ProductResponse> GetAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("PRODUCT_NOT_FOUND");

            return MapToResponse(product, product.Category.Name);
        }

        public async Task<List<ProductResponse>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return products.Select(p => MapToResponse(p, p.Category.Name)).ToList();
        }

        public async Task<List<ProductResponse>> GetByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsAvailable)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return products.Select(p => MapToResponse(p, p.Category.Name)).ToList();
        }

        public async Task<List<ProductResponse>> GetByRestaurantAsync(Guid restaurantId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.RestaurantId == restaurantId && p.IsAvailable)
                .OrderBy(p => p.Category.Name)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return products.Select(p => MapToResponse(p, p.Category.Name)).ToList();
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("PRODUCT_NOT_FOUND");

            if (request.Name != null) product.Name = request.Name;
            if (request.Description != null) product.Description = request.Description;
            if (request.Price != null) product.Price = request.Price.Value;
            if (request.WeightKg != null) product.WeightKg = request.WeightKg.Value;
            if (request.IsAvailable != null) product.IsAvailable = request.IsAvailable.Value;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(product, product.Category.Name);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("PRODUCT_NOT_FOUND");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private static ProductResponse MapToResponse(Product p, string categoryName) => new()
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            CategoryName = categoryName,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            WeightKg = p.WeightKg,
            IsAvailable = p.IsAvailable,
            CreatedAt = p.CreatedAt
        };
    }
}
