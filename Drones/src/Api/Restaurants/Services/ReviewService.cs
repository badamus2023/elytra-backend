using Drones.src.Api.Data;
using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Restaurants.Services
{
    public class ReviewService : IReviewService
    {

        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewResponse> CreateAsync(Guid userId, CreateReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var exists = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.RestaurantId == request.RestaurantId);

            if (exists)
                throw new InvalidOperationException("You have already reviewd this restaurant.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException("USER_NOT_FOUND");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RestaurantId = request.RestaurantId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return MapToResponse(review, user.Email);
        }

        public async Task DeleteAsync(Guid reviewId, Guid userId, bool isAdmin = false)
        {
            var review = isAdmin
                ? await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId)
                : await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

            if (review is null)
                throw new InvalidOperationException("REVIEW_NOT_FOUND");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReviewResponse>> GetByRestaurantAsync(Guid restaurantId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => MapToResponse(r, r.User.Email)).ToList();
        }

        public async Task<ReviewResponse> UpdateAsync(Guid reviewId, Guid userId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId)
                ?? throw new InvalidOperationException("REVIEW_NOT_FOUND");

            review.Rating = rating;
            review.Comment = comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(review, review.User.Email);
        }

        //helpers

        private static ReviewResponse MapToResponse(Review r, string email) => new()
        {
            Id = r.Id,
            UserId = r.UserId,
            UserEmail = email,
            RestaurantId = r.RestaurantId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
