using Drones.src.Api.Payments.DTOs.Responses;

namespace Drones.src.Api.Payments.Services
{
    public interface IPaymentService
    {
        Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(Guid orderId, Guid userId);
        Task<PaymentStatusResponse> ConfirmCheckoutSessionAsync(string sessionId, Guid userId);
        Task HandleWebhookAsync(string json, string signatureHeader);
    }
}
