using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.DeliveryPoints.DTOs.Requests;
using Drones.src.Api.DeliveryPoints.DTOs.Responses;
using Drones.src.Api.DeliveryPoints.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.DeliveryPoints.Controllers
{
    [ApiController]
    [Route("api/delivery-points")]
    public class DeliveryPointController : ControllerBase
    {
        private readonly IDeliveryPointService _deliveryPointService;

        public DeliveryPointController(IDeliveryPointService deliveryPointService)
        {
            _deliveryPointService = deliveryPointService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeliveryPointResponse>>> GetActive()
        {
            var result = await _deliveryPointService.GetActiveAsync();
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DeliveryPointResponse>>> GetAll()
        {
            var result = await _deliveryPointService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryPointResponse>> Get(Guid id)
        {
            var result = await _deliveryPointService.GetAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeliveryPointResponse>> Create(CreateDeliveryPointRequest request)
        {
            var result = await _deliveryPointService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeliveryPointResponse>> Update(Guid id, UpdateDeliveryPointRequest request)
        {
            var result = await _deliveryPointService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> Deactivate(Guid id)
        {
            await _deliveryPointService.DeactivateAsync(id);
            return Ok(new MessageResponse { Message = "Delivery point deactivated." });
        }
    }
}
