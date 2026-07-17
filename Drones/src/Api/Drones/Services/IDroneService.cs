using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;

namespace Drones.src.Api.Drones.Services
{
    public interface IDroneService
    {
        Task<DroneResponse> CreateDroneAsync(CreateDroneRequest request);
        Task<DroneResponse> GetDroneAsync(Guid droneId);
        Task<List<DroneResponse>> GetAllDronesAsync();
        Task<List<DroneResponse>> GetAvailableDronesAsync();
        Task<DroneResponse> UpdateDroneAsync(Guid droneId, UpdateDroneRequest request);
        Task DeleteDroneAsync(Guid droneId);
        Task UpdateDroneLocationAsync(Guid droneId, double latitude, double longitude);
    }
}
