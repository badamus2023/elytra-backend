using Drones.src.Api.Data;
using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Restaurants.Services
{
    public class RestaurantService : IRestaurantService
    {

        private readonly AppDbContext _context;

        public RestaurantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request)
        {
            var restaurant = new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                isOpen = true,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();

            return MapToResponse(restaurant, 0, 0);
        }

        public async Task DeleteAsync(Guid id)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id)
                 ?? throw new InvalidOperationException("RESTAURANT_NOT_FOUND");

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RestaurantResponse>> GetAllAsync()
        {
            var restaurants = await _context.Restaurants.Include(r => r.Reviews).OrderBy(r => r.Name)
                .ToListAsync();

            return restaurants.Select(r =>
            {
                var avg = r.Reviews.Any() ? r.Reviews.Average(rv => rv.Rating) : 0;
                return MapToResponse(r, avg, r.Reviews.Count());
            }).ToList();
        }

        public async Task<RestaurantResponse> GetAsync(Guid id)
        {
            var restaurant = await _context.Restaurants.Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.Id == id) ?? throw new InvalidOperationException("RESTAURANT_NOT_FOUND");

            var avgRating = restaurant.Reviews.Any()
                ? restaurant.Reviews.Average(r => r.Rating) : 0;

            return MapToResponse(restaurant, avgRating, restaurant.Reviews.Count());
        }

        public async Task<RestaurantResponse> UpdateAsync(Guid id, UpdateRestaurantRequest request)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new InvalidOperationException("RESTAURANT_NOT_FOUND");

            if (request.Name != null) restaurant.Name = request.Name;
            if (request.Address != null) restaurant.Address = request.Address;
            if (request.Description != null) restaurant.Description = request.Description;
            if (request.ImageUrl != null) restaurant.ImageUrl = request.ImageUrl;
            if (request.IsOpen != null) restaurant.isOpen = request.IsOpen.Value;
            if (request.OpenTime != null) restaurant.OpenTime = request.OpenTime.Value;
            if (request.CloseTime != null) restaurant.CloseTime = request.CloseTime.Value;
            restaurant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var avg = restaurant.Reviews.Any()
                ? restaurant.Reviews.Average(r => r.Rating) : 0;

            return MapToResponse(restaurant, avg, restaurant.Reviews.Count);
        }

        //helpers

        private static RestaurantResponse MapToResponse( Restaurant r, double avgRating, int reviewCount) => new()
        {
            Id = r.Id,
            Name = r.Name,
            Address = r.Address,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            IsOpen = r.isOpen,
            Description = r.Description,
            ImageUrl = r.ImageUrl,
            OpenTime = r.OpenTime,
            CloseTime = r.CloseTime,
            AverageRating = Math.Round(avgRating, 1),
            ReviewCount = reviewCount,
            CreatedAt = r.CreatedAt
        };
    }
}
