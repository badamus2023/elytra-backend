namespace Drones.src.Api.Orders.DTOs.Requests
{
    public class CreateOrderRequest
    {
        public Guid RestaurantId { get; set; }
        public Guid? DeliveryPointId { get; set; }
        public string? DeliveryAddress { get; set; }
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
        public List<OrderItemRequest> Items { get; set; } = [];
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
