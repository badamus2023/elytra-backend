using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Orders.DTOs.Requests;
using Drones.src.Api.Orders.DTOs.Responses;
using Drones.src.Api.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drones.src.Api.Orders.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController: ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
        {
            var userId = GetUserId();
            var result = await _orderService.CreateOrderAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(Guid orderId)
        {
            var userId = GetUserId();
            var result = User.IsInRole("Admin")
                ? await _orderService.GetOrderByIdAsync(orderId)
                : await _orderService.GetOrderAsync(orderId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetMyOrders()
        {
            var userId = GetUserId();
            var result = User.IsInRole("Admin")
                ? await _orderService.GetAllOrdersAsync()
                : User.IsInRole("RestaurantOwner")
                    ? await _orderService.GetRestaurantOwnerOrdersAsync(userId)
                    : await _orderService.GetUserOrdersAsync(userId);
            return Ok(result);
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult<MessageResponse>> CancelOrder(Guid orderId)
        {
            var userId = GetUserId();
            await _orderService.CancelOrderAsync(orderId, userId);
            return Ok(new { message = "Order cancelled" });
        }

        [HttpPost("{orderId}/confirm-receipt")]
        public async Task<ActionResult<OrderResponse>> ConfirmReceipt(Guid orderId)
        {
            var result = await _orderService.ConfirmReceiptAsync(orderId, GetUserId());
            return Ok(result);
        }

        //helpers

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("User not authenticated");

            return Guid.Parse(claim);
        }
    }
}
