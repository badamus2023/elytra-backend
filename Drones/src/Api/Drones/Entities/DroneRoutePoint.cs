using Drones.src.Api.Dispatches.Entities;

namespace Drones.src.Api.Drones.Entities
{
    public class DroneRoutePoint
    {
        public long Id { get; set; }
        public Guid DispatchId { get; set; }
        public Guid DroneId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BatteryLevel { get; set; }
        public DateTime RecordedAt { get; set; }
        public Dispatch Dispatch { get; set; } = default!;
    }
}
