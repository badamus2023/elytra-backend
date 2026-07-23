using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drones.src.Api.Restaurants.Controllers
{
    [ApiController]
    [Route("api/restaurant-applications")]
    [Authorize]
    public class RestaurantApplicationController : ControllerBase
    {
        private readonly IRestaurantApplicationService _service;
        public RestaurantApplicationController(IRestaurantApplicationService service) =>
            _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public Task<List<RestaurantApplicationResponse>> GetAll() => _service.GetAllAsync();

        [HttpGet("mine")]
        [Authorize(Roles = "RestaurantOwner")]
        public Task<RestaurantApplicationResponse> GetMine() =>
            _service.GetMineAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public Task<RestaurantApplicationResponse> Approve(
            Guid id, ReviewRestaurantApplicationRequest request) =>
            _service.ApproveAsync(id, request.AdminNote);

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public Task<RestaurantApplicationResponse> Reject(
            Guid id, ReviewRestaurantApplicationRequest request) =>
            _service.RejectAsync(id, request.AdminNote);
    }
}
