using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drones.src.Api.Restaurants.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RestaurantResponse>> Create(CreateRestaurantRequest request)
        {
            var result = await _restaurantService.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<RestaurantResponse>>> GetAll()
        {
            var result = await _restaurantService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantResponse>> Get(Guid id)
        {
            var result = await _restaurantService.GetAsync(id);
            return Ok(result);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "RestaurantOwner")]
        public Task<RestaurantResponse> GetMine() =>
            _restaurantService.GetMineAsync(GetUserId());

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<ActionResult<RestaurantResponse>> Update(Guid id, UpdateRestaurantRequest request)
        {
            var result = User.IsInRole("Admin")
                ? await _restaurantService.UpdateAsync(id, request)
                : await _restaurantService.UpdateOwnedAsync(id, GetUserId(), request);
            return Ok(result);
        }

        private Guid GetUserId() => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> Delete(Guid id)
        {
            await _restaurantService.DeleteAsync(id);
            return Ok(new MessageResponse { Message = "Restaurant deleted." });
        }
    }
}
