using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Products.DTOs.Requests;
using Drones.src.Api.Products.DTOs.Responses;
using Drones.src.Api.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.Products.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
        {
            var result = await _productService.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> Get(Guid id)
        {
            var result = await _productService.GetAsync(id);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductResponse>>> GetByCategory(int categoryId)
        {
            var result = await _productService.GetByCategoryAsync(categoryId);
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<List<ProductResponse>>> GetByRestaurant(Guid restaurantId)
        {
            var result = await _productService.GetByRestaurantAsync(restaurantId);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request)
        {
            var result = await _productService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new MessageResponse { Message = "Product deleted." });
        }
    }
}
