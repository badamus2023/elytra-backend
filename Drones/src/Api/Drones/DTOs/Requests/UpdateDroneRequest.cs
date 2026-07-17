namespace Drones.src.Api.Drones.DTOs.Requests
{
    public class UpdateDroneRequest
    {
        public string? Name { get; set; }
        public double? MaxPayloadKg { get; set; }
        public double? BatteryLevel { get; set; }
    }
}
