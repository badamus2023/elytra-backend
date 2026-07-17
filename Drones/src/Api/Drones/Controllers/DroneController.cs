using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;
using Drones.src.Api.Drones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.Drones.Controllers
{
    [ApiController]
    [Route("api/drones")]
    [Authorize]
    public class DroneController: ControllerBase
    {
        private readonly IDroneService _droneService;

        public DroneController(IDroneService droneService)
        {
            _droneService = droneService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DroneResponse>> CreateDrone(CreateDroneRequest request)
        {
            var result = await _droneService.CreateDroneAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DroneResponse>>> GetAllDrones()
        {
            var result = await _droneService.GetAllDronesAsync();
            return Ok(result);
        }

        [HttpGet("available")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DroneResponse>>> GetAvailableDrones()
        {
            var result = await _droneService.GetAvailableDronesAsync();
            return Ok(result);
        }

        [HttpGet("{droneId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DroneResponse>> GetDrone(Guid droneId)
        {
            var result = await _droneService.GetDroneAsync(droneId);
            return Ok(result);
        }

        [HttpPatch("{droneId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DroneResponse>> UpdateDrone(Guid droneId, UpdateDroneRequest request)
        {
            var result = await _droneService.UpdateDroneAsync(droneId, request);
            return Ok(result);
        }

        [HttpDelete("{droneId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponse>> DeleteDrone(Guid droneId)
        {
            await _droneService.DeleteDroneAsync(droneId);
            return Ok(new { message = "Drone deleted." });
        }
    }
}
