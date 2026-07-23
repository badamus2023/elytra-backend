using Drones.src.Api.DeliveryPoints.Entities;
using Drones.src.Api.Dispatches.Entities;
using Drones.src.Api.Payments.Entities;
using Drones.src.Api.Restaurants.Entities;

namespace Drones.src.Api.Orders.Entities
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Dispatched,
        InFlight,
        Delivered,
        Cancelled,
        Completed
    }

    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RestaurantId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Guid? DeliveryPointId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public Restaurant Restaurant { get; set; } = default!;
        public DeliveryPoint? DeliveryPoint { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
        public Dispatch? Dispatch { get; set; }
    }
}
