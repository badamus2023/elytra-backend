namespace Drones.src.Api.Drones.DTOs.Responses
{
    public class DroneResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double BatteryLevel { get; set; }
        public double MaxPayloadKg { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
