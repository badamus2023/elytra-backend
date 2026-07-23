using Drones.src.Api.Common.DTOs;

namespace Drones.src.Api.Common.Services
{
    public interface IRealtimeNotificationService
    {
        Task NotifyUserAsync(Guid userId, NotificationMessage notification);
        Task NotifyRoleAsync(string role, NotificationMessage notification);
    }
}
