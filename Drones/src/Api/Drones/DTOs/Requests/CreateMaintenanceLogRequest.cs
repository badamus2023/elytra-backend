namespace Drones.src.Api.Drones.DTOs.Requests
{
    public class CreateMaintenanceLogRequest
    {
        public Guid DroneId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
    }
}
