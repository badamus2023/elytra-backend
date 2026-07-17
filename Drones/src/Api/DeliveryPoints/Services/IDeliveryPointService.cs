using Drones.src.Api.DeliveryPoints.DTOs.Requests;
using Drones.src.Api.DeliveryPoints.DTOs.Responses;

namespace Drones.src.Api.DeliveryPoints.Services
{
    public interface IDeliveryPointService
    {
        Task<List<DeliveryPointResponse>> GetActiveAsync();
        Task<List<DeliveryPointResponse>> GetAllAsync();
        Task<DeliveryPointResponse> GetAsync(Guid id);
        Task<DeliveryPointResponse> CreateAsync(CreateDeliveryPointRequest request);
        Task<DeliveryPointResponse> UpdateAsync(Guid id, UpdateDeliveryPointRequest request);
        Task DeactivateAsync(Guid id);
    }
}
