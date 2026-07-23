using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Drones.src.Api.Common.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub");

            if (!string.IsNullOrWhiteSpace(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

            foreach (var role in Context.User?.FindAll(ClaimTypes.Role) ?? [])
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{role.Value}");

            await base.OnConnectedAsync();
        }
    }
}
