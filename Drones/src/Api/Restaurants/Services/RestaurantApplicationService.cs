using Drones.src.Api.Common.DTOs;
using Drones.src.Api.Common.Services;
using Drones.src.Api.Data;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Restaurants.Services
{
    public class RestaurantApplicationService : IRestaurantApplicationService
    {
        private readonly AppDbContext _db;
        private readonly IRealtimeNotificationService _notifications;
        public RestaurantApplicationService(
            AppDbContext db,
            IRealtimeNotificationService notifications)
        {
            _db = db;
            _notifications = notifications;
        }

        public async Task<List<RestaurantApplicationResponse>> GetAllAsync() =>
            (await _db.RestaurantApplications.Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt).ToListAsync()).Select(Map).ToList();

        public async Task<RestaurantApplicationResponse> GetMineAsync(Guid userId) =>
            Map(await _db.RestaurantApplications.Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.UserId == userId)
                ?? throw new InvalidOperationException("APPLICATION_NOT_FOUND"));

        public Task<RestaurantApplicationResponse> ApproveAsync(Guid id, string? note) =>
            ReviewAsync(id, true, note);

        public Task<RestaurantApplicationResponse> RejectAsync(Guid id, string? note) =>
            ReviewAsync(id, false, note);

        private async Task<RestaurantApplicationResponse> ReviewAsync(
            Guid id, bool approve, string? note)
        {
            var application = await _db.RestaurantApplications.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("APPLICATION_NOT_FOUND");
            if (application.Status != RestaurantApplicationStatus.Pending)
                throw new InvalidOperationException("Application was already reviewed.");

            if (approve)
            {
                var restaurant = new Restaurant
                {
                    Id = Guid.NewGuid(),
                    OwnerUserId = application.UserId,
                    Name = application.RestaurantName,
                    Address = application.Address,
                    Latitude = application.Latitude,
                    Longitude = application.Longitude,
                    Description = application.Description,
                    OpenTime = application.OpenTime,
                    CloseTime = application.CloseTime,
                    isOpen = false,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Restaurants.Add(restaurant);
                application.RestaurantId = restaurant.Id;
                application.Status = RestaurantApplicationStatus.Approved;
            }
            else
            {
                application.Status = RestaurantApplicationStatus.Rejected;
            }
            application.AdminNote = note?.Trim();
            application.ReviewedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _notifications.NotifyUserAsync(application.UserId, new NotificationMessage
            {
                Category = "restaurant",
                Severity = approve ? "success" : "warning",
                Title = approve ? "Restaurant application approved" : "Restaurant application rejected",
                Message = approve
                    ? "Your restaurant is ready to configure."
                    : application.AdminNote ?? "Contact support for more information."
            });
            return Map(application);
        }

        private static RestaurantApplicationResponse Map(RestaurantApplication x) => new()
        {
            Id = x.Id, UserId = x.UserId, Email = x.User.Email,
            CompanyName = x.CompanyName, TaxId = x.TaxId,
            ContactPhone = x.ContactPhone, RestaurantName = x.RestaurantName,
            Address = x.Address, Latitude = x.Latitude, Longitude = x.Longitude,
            Description = x.Description, OpenTime = x.OpenTime, CloseTime = x.CloseTime,
            Status = x.Status.ToString(), AdminNote = x.AdminNote,
            CreatedAt = x.CreatedAt, ReviewedAt = x.ReviewedAt,
            RestaurantId = x.RestaurantId
        };
    }
}
