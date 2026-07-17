using Drones.src.Api.Data;
using Drones.src.Api.Orders.DTOs.Requests;
using Drones.src.Api.Orders.DTOs.Responses;
using Drones.src.Api.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using static Drones.src.Api.Orders.DTOs.Responses.OrderResponse;

namespace Drones.src.Api.Orders.Services
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CancelOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new InvalidOperationException("ORDER_NOT_FOUND");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<OrderResponse> CreateOrderAsync(Guid userId, CreateOrderRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Order must have at least one item.");

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == request.RestaurantId)
                ?? throw new InvalidOperationException("RESTAURANT_NOT_FOUND");

            if (!restaurant.isOpen)
                throw new InvalidOperationException("Restaurant is currently closed.");

            var productIds = request.Items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.Id)
                    && p.Category.RestaurantId == request.RestaurantId
                    && p.IsAvailable)
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new InvalidOperationException(
                    "One or more products not found or unavailable in this restaurant.");

            var orderItems = request.Items.Select(i =>
            {
                var product = products.First(p => p.Id == i.ProductId);
                return new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = i.Quantity
                };
            }).ToList();

            var total = orderItems.Sum(i => i.UnitPrice * i.Quantity);

            string deliveryAddress;
            double deliveryLatitude;
            double deliveryLongitude;
            Guid? deliveryPointId = null;

            if (request.DeliveryPointId.HasValue && request.DeliveryPointId.Value != Guid.Empty)
            {
                var point = await _context.DeliveryPoints
                    .FirstOrDefaultAsync(p => p.Id == request.DeliveryPointId.Value && p.IsActive)
                    ?? throw new InvalidOperationException("DELIVERY_POINT_NOT_FOUND");

                deliveryPointId = point.Id;
                deliveryLatitude = point.Latitude;
                deliveryLongitude = point.Longitude;
                var baseAddress = $"{point.Name} — {point.Address}";
                var notes = request.DeliveryAddress?.Trim();
                deliveryAddress = string.IsNullOrEmpty(notes)
                    ? baseAddress
                    : $"{baseAddress} ({notes})";
            }
            else if (!string.IsNullOrWhiteSpace(request.DeliveryAddress))
            {
                deliveryAddress = request.DeliveryAddress.Trim();
                deliveryLatitude = request.DeliveryLatitude;
                deliveryLongitude = request.DeliveryLongitude;

                if (deliveryLatitude == 0 && deliveryLongitude == 0)
                    throw new InvalidOperationException("Delivery coordinates are required when no pickup point is selected.");
            }
            else
            {
                throw new InvalidOperationException("Select a pickup point or provide a delivery address.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RestaurantId = request.RestaurantId,
                DeliveryPointId = deliveryPointId,
                Status = OrderStatus.Pending,
                DeliveryAddress = deliveryAddress,
                DeliveryLatitude = deliveryLatitude,
                DeliveryLongitude = deliveryLongitude,
                TotalAmount = total,
                CreatedAt = DateTime.UtcNow,
                Items = orderItems
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return MapToResponse(order);
        }

        public async Task<OrderResponse> GetOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new InvalidOperationException("ORDER_NOT_FOUND");

            return MapToResponse(order);
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new InvalidOperationException("ORDER_NOT_FOUND");

            return MapToResponse(order);
        }

        public async Task<List<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponse).ToList();
        }

        public async Task<List<OrderResponse>> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _context.Orders.Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponse).ToList();
        }

        //helpers

        private static OrderResponse MapToResponse(Order order) => new()
        {
            Id = order.Id,
            UserId = order.UserId,
            RestaurantId = order.RestaurantId,
            DeliveryPointId = order.DeliveryPointId,
            Status = order.Status.ToString(),
            DeliveryAddress = order.DeliveryAddress,
            DeliveryLatitude = order.DeliveryLatitude,
            DeliveryLongitude = order.DeliveryLongitude,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
            }).ToList()
        };
    }
}
