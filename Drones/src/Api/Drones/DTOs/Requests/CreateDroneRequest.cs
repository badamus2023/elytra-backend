namespace Drones.src.Api.Drones.DTOs.Requests
{
    public class CreateDroneRequest
    {
        public string Name { get; set; } = string.Empty;
        public double MaxPayloadKg {  get; set; }
        public double HomeLatitude { get; set; }
        public double HomeLongitude { get; set; }
    }
}
