using Microsoft.AspNetCore.SignalR;

namespace Drones.src.Api.Common.Hubs
{
    public class DroneHub: Hub
    {
        public async Task SubscribeToDrone(string droneId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"drone:{droneId}");
        }

        public async Task UnsubscribeFromDrone(string droneId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"drone:{droneId}");
        }
    }
}
