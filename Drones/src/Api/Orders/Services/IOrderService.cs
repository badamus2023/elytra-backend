using Drones.src.Api.Orders.DTOs.Requests;
using Drones.src.Api.Orders.DTOs.Responses;

namespace Drones.src.Api.Orders.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(Guid userId, CreateOrderRequest request);
        Task<OrderResponse> GetOrderAsync(Guid orderId, Guid userId);
        Task<OrderResponse> GetOrderByIdAsync(Guid orderId);
        Task<List<OrderResponse>> GetUserOrdersAsync(Guid userId);
        Task<List<OrderResponse>> GetAllOrdersAsync();
        Task CancelOrderAsync(Guid orderId, Guid userId);
    }
}
