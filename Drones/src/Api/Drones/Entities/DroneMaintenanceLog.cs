namespace Drones.src.Api.Drones.Entities
{
    public enum MaintenanceType
    {
        Battery,
        Motor,
        Software,
        General
    }

    public class DroneMaintenanceLog
    {
        public Guid Id { get; set; }
        public Guid DroneId { get; set; }
        public MaintenanceType Type { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Drone Drone { get; set; } = default!;
    }
}
