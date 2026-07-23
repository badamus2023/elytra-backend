namespace Drones.src.Api.Dispatches.DTOs.Responses
{
    public class DroneRoutePointResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BatteryLevel { get; set; }
        public DateTime RecordedAt { get; set; }
    }

    public class DroneFlightHistoryResponse
    {
        public Guid DispatchId { get; set; }
        public Guid OrderId { get; set; }
        public Guid DroneId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public List<DroneRoutePointResponse> RoutePoints { get; set; } = [];
    }
}
