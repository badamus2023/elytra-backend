using Drones.src.Api.Drones.Entities;
using Drones.src.Api.Orders.Entities;

namespace Drones.src.Api.Dispatches.Entities
{
    public enum DispatchStatus
    {
        Assigned,
        InFlight,
        Delivered,
        Failed
    }

    public class Dispatch
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid DroneId { get; set; }

        public DispatchStatus Status { get; set; } = DispatchStatus.Assigned;

        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public string? RouteData { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Order Order { get; set; } = default!;
        public Drone Drone { get; set; } = default!;
        public ICollection<DroneRoutePoint> RoutePoints { get; set; } = new List<DroneRoutePoint>();
    }
}
