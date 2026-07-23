using Drones.src.Api.Common.DTOs;
using Drones.src.Api.Common.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Drones.src.Api.Common.Services
{
    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private const string ClientEvent = "NotificationReceived";
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealtimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyUserAsync(Guid userId, NotificationMessage notification) =>
            _hubContext.Clients.Group($"user:{userId}").SendAsync(ClientEvent, notification);

        public Task NotifyRoleAsync(string role, NotificationMessage notification) =>
            _hubContext.Clients.Group($"role:{role}").SendAsync(ClientEvent, notification);
    }
}
