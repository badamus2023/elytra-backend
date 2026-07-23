using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;
using Drones.src.Api.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Drones.src.Api.Restaurants.Services;
using System.Security.Claims;

namespace Drones.src.Api.Products.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IRestaurantOwnershipService _ownership;

        public CategoryController(ICategoryService categoryService, IRestaurantOwnershipService ownership)
        {
            _categoryService = categoryService;
            _ownership = ownership;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
        {
            await EnsureOwnerRestaurant(request.RestaurantId);
            var result = await _categoryService.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> Get(int id)
        {
            var result = await _categoryService.GetAsync(id);
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<List<CategoryResponse>>> GetByRestaurant(Guid restaurantId)
        {
            var result = await _categoryService.GetByRestaurantAsync(restaurantId);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<ActionResult<CategoryResponse>> Update(int id, UpdateCategoryRequest request)
        {
            await EnsureOwnerCategory(id);
            var result = await _categoryService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<ActionResult<MessageResponse>> Delete(int id)
        {
            await EnsureOwnerCategory(id);
            await _categoryService.DeleteAsync(id);
            return Ok(new MessageResponse { Message = "Category deleted." });
        }

        private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private Task EnsureOwnerRestaurant(Guid id) => User.IsInRole("RestaurantOwner") ? _ownership.EnsureRestaurantAsync(UserId(), id) : Task.CompletedTask;
        private Task EnsureOwnerCategory(int id) => User.IsInRole("RestaurantOwner") ? _ownership.EnsureCategoryAsync(UserId(), id) : Task.CompletedTask;
    }
}
