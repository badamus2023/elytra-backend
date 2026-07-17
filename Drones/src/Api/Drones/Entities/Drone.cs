using Drones.src.Api.Dispatches.Entities;

namespace Drones.src.Api.Drones.Entities
{
    public enum DroneStatus
    {
        Idle,
        Assigned,
        InFlight,
        Maintenance
    }

    public class Drone
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public DroneStatus Status { get; set; } = DroneStatus.Idle;

        public double BatteryLevel { get; set; } = 100;
        public double MaxPayloadKg { get; set; }

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }

        public DateTime? LastSeenAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Dispatch> Dispatches { get; set; } = new List<Dispatch>();
        public ICollection<DroneMaintenanceLog> MaintenanceLogs { get; set; } = new List<DroneMaintenanceLog>();
    }
}
