using Drones.src.Api.Orders.Entities;

namespace Drones.src.Api.Payments.Entities
{

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentProvider
    {
        Stripe,
        Przelewy24
    }

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PLN";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentProvider Provider { get; set; }

        public string? ProviderPaymentId { get; set; }
        public string? ProviderSessionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Order Order { get; set; } = default!;
    }
}
