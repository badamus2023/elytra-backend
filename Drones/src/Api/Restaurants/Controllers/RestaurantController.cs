using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RestaurantResponse>> Update(Guid id, UpdateRestaurantRequest request)
        {
            var result = await _restaurantService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> Delete(Guid id)
        {
            await _restaurantService.DeleteAsync(id);
            return Ok(new MessageResponse { Message = "Restaurant deleted." });
        }
    }
}
