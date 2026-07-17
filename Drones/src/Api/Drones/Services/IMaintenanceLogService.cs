using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;

namespace Drones.src.Api.Drones.Services
{
    public interface IMaintenanceLogService
    {
        Task<MaintenanceLogResponse> CreateAsync(CreateMaintenanceLogRequest request);
        Task<MaintenanceLogResponse> GetAsync(Guid id);
        Task<List<MaintenanceLogResponse>> GetByDroneAsync(Guid droneId);
        Task<MaintenanceLogResponse> UpdateAsync(Guid id, UpdateMaintenanceLogRequest request);
        Task DeleteAsync(Guid id);
    }
}
