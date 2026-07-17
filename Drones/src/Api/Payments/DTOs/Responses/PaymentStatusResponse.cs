namespace Drones.src.Api.Payments.DTOs.Responses
{
    public class PaymentStatusResponse
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PLN";
    }
}
