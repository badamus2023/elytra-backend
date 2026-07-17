using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;
using Drones.src.Api.Drones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drones.src.Api.Drones.Controllers
{
    [ApiController]
    [Route("api/maintenance-logs")]
    [Authorize(Roles = "Admin")]
    public class MaintenanceLogController: ControllerBase
    {
        private readonly IMaintenanceLogService _maintenanceLogService;

        public MaintenanceLogController(IMaintenanceLogService maintenanceLogService)
        {
            _maintenanceLogService = maintenanceLogService;
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceLogResponse>> Create(CreateMaintenanceLogRequest request)
        {
            var result = await _maintenanceLogService.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceLogResponse>> Get(Guid id)
        {
            var result = await _maintenanceLogService.GetAsync(id);
            return Ok(result);
        }

        [HttpGet("drone/{droneId}")]
        public async Task<ActionResult<List<MaintenanceLogResponse>>> GetByDrone(Guid droneId)
        {
            var result = await _maintenanceLogService.GetByDroneAsync(droneId);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<MaintenanceLogResponse>> Update(Guid id, UpdateMaintenanceLogRequest request)
        {
            var result = await _maintenanceLogService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<MessageResponse>> Delete(Guid id)
        {
            await _maintenanceLogService.DeleteAsync(id);
            return Ok(new { message = "Log deleted." });
        }
    }
}
