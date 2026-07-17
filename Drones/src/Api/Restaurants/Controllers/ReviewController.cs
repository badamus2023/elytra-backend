using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Auth.Entities;
using Drones.src.Api.Restaurants.DTOs.Requests;
using Drones.src.Api.Restaurants.DTOs.Responses;
using Drones.src.Api.Restaurants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drones.src.Api.Restaurants.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponse>> Create(CreateReviewRequest request)
        {
            var userId = GetUserId();
            var result = await _reviewService.CreateAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("restaurant/{restaurantId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReviewResponse>>> GetByRestaurant(Guid restaurantId)
        {
            var result = await _reviewService.GetByRestaurantAsync(restaurantId);
            return Ok(result);
        }

        [HttpPatch("{reviewId}")]
        public async Task<ActionResult<ReviewResponse>> Update(
            Guid reviewId, UpdateReviewRequest request)
        {
            var userId = GetUserId();
            var result = await _reviewService.UpdateAsync(
                reviewId, userId, request.Rating, request.Comment);
            return Ok(result);
        }

        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<MessageResponse>> Delete(Guid reviewId)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");
            await _reviewService.DeleteAsync(reviewId, userId, isAdmin);
            return Ok(new MessageResponse { Message = "Review deleted." });
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("User not authenticated.");
            return Guid.Parse(claim);
        }
    }
}
