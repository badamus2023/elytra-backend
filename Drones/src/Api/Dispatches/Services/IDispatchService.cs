using Drones.src.Api.Dispatches.DTOs.Requests;
using Drones.src.Api.Dispatches.DTOs.Responses;

namespace Drones.src.Api.Dispatches.Services
{
    public interface IDispatchService
    {
        Task<DispatchResponse> CreateDispatchAsync(CreateDispatchRequest request);
        Task<DispatchResponse> GetDispatchAsync(Guid dispatchId);
        Task<DispatchResponse> GetDispatchByOrderAsync(Guid orderId);
        Task<DispatchResponse> UpdateDispatchStatusAsync(Guid dispatchId, string status);
        Task SimulateFlightAsync(Guid dispatchId);
    }
}
