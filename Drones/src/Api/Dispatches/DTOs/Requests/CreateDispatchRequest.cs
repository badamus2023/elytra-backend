namespace Drones.src.Api.Dispatches.DTOs.Requests
{
    public class CreateDispatchRequest
    {
        public Guid OrderId { get; set; }
        public Guid? DroneId { get; set; }
    }
}
