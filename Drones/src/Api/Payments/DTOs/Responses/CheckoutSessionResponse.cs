namespace Drones.src.Api.Payments.DTOs.Responses
{
    public class CheckoutSessionResponse
    {
        public Guid OrderId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
