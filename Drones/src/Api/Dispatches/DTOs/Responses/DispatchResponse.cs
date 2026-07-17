namespace Drones.src.Api.Dispatches.DTOs.Responses
{
    public class DispatchResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid DroneId { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? EstimatedDeliveryMinutes { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public double DroneLatitude { get; set; }
        public double DroneLongitude { get; set; }
    }
}
