namespace Drones.src.Api.Drones.DTOs.Responses
{
    public class MaintenanceLogResponse
    {
        public Guid Id { get; set; }
        public Guid DroneId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
