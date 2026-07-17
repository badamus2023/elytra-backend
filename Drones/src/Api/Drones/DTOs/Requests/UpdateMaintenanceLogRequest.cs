namespace Drones.src.Api.Drones.DTOs.Requests
{
    public class UpdateMaintenanceLogRequest
    {
        public string? Notes { get; set; }
        public DateTime? PerformedAt { get; set; }
    }
}
