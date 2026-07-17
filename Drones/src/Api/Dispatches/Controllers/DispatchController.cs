using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Dispatches.DTOs.Requests;
using Drones.src.Api.Dispatches.DTOs.Responses;
using Drones.src.Api.Dispatches.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.Dispatches.Controllers
{
    [ApiController]
    [Route("api/dispatches")]
    [Authorize]
    public class DispatchController : ControllerBase
    {
        private readonly IDispatchService _dispatchService;

        public DispatchController(IDispatchService dispatchService)
        {
            _dispatchService = dispatchService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DispatchResponse>> CreateDispatch(CreateDispatchRequest request)
        {
            var result = await _dispatchService.CreateDispatchAsync(request);
            return Ok(result);
        }

        [HttpGet("{dispatchId}")]
        public async Task<ActionResult<DispatchResponse>> GetDispatch(Guid dispatchId)
        {
            var result = await _dispatchService.GetDispatchAsync(dispatchId);
            return Ok(result);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<DispatchResponse>> GetDispatchByOrder(Guid orderId)
        {
            var result = await _dispatchService.GetDispatchByOrderAsync(orderId);
            return Ok(result);
        }

        [HttpPatch("{dispatchId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DispatchResponse>> UpdateStatus(Guid dispatchId, [FromBody] string status)
        {
            var result = await _dispatchService.UpdateDispatchStatusAsync(dispatchId, status);
            return Ok(result);
        }

        [HttpPost("{dispatchId}/simulate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> SimulateFlight(Guid dispatchId)
        {
            await _dispatchService.SimulateFlightAsync(dispatchId);
            return Ok(new { message = "Simulation started." });
        }
    }
}
