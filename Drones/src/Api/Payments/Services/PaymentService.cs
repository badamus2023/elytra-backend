using Drones.src.Api.Data;
using Drones.src.Api.Orders.Entities;
using Drones.src.Api.Common.DTOs;
using Drones.src.Api.Common.Services;
using Drones.src.Api.Payments.DTOs.Responses;
using Drones.src.Api.Payments.Entities;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Drones.src.Api.Payments.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly StripeClient _stripeClient;
        private readonly IConfiguration _configuration;
        private readonly IRealtimeNotificationService _notifications;

        public PaymentService(
            AppDbContext context,
            StripeClient stripeClient,
            IConfiguration configuration,
            IRealtimeNotificationService notifications)
        {
            _context = context;
            _stripeClient = stripeClient;
            _configuration = configuration;
            _notifications = notifications;
        }

        public async Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(Guid orderId, Guid userId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new InvalidOperationException("ORDER_NOT_FOUND");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot pay for a cancelled order.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order is already paid or in progress.");

            if (order.Items.Count == 0)
                throw new InvalidOperationException("Order has no items.");

            var frontendUrl = (_configuration["Stripe:FrontendUrl"]
                ?? throw new InvalidOperationException("Stripe:FrontendUrl is not configured"))
                .TrimEnd('/');

            var payment = order.Payment;
            if (payment is null)
            {
                payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = userId,
                    Amount = order.TotalAmount,
                    Currency = "PLN",
                    Status = PaymentStatus.Pending,
                    Provider = PaymentProvider.Stripe,
                    CreatedAt = DateTime.UtcNow,
                };
                await _context.Payments.AddAsync(payment);
            }
            else if (payment.Status == PaymentStatus.Completed)
            {
                throw new InvalidOperationException("Order is already paid.");
            }
            else
            {
                payment.Amount = order.TotalAmount;
                payment.UpdatedAt = DateTime.UtcNow;
            }

            var lineItems = order.Items.Select(item => new SessionLineItemOptions
            {
                Quantity = item.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = payment.Currency.ToLowerInvariant(),
                    UnitAmountDecimal = item.UnitPrice * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName,
                    },
                },
            }).ToList();

            var sessionOptions = new SessionCreateOptions
            {
                Mode = "payment",
                ClientReferenceId = order.Id.ToString(),
                SuccessUrl = $"{frontendUrl}/order-confirmed?orderId={order.Id}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{frontendUrl}/checkout?restaurantId={order.RestaurantId}&payment=cancelled",
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = order.Id.ToString(),
                    ["userId"] = userId.ToString(),
                },
                LineItems = lineItems,
            };

            var sessionService = new SessionService(_stripeClient);
            var session = await sessionService.CreateAsync(sessionOptions);

            payment.ProviderSessionId = session.Id;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CheckoutSessionResponse
            {
                OrderId = order.Id,
                SessionId = session.Id,
                CheckoutUrl = session.Url
                    ?? throw new InvalidOperationException("Stripe did not return a checkout URL."),
            };
        }

        public async Task<PaymentStatusResponse> ConfirmCheckoutSessionAsync(string sessionId, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new InvalidOperationException("Session id is required.");

            var sessionService = new SessionService(_stripeClient);
            var session = await sessionService.GetAsync(sessionId);

            if (session.PaymentStatus != "paid")
                throw new InvalidOperationException("Payment is not completed yet.");

            await MarkOrderPaidFromSessionAsync(session, userId);

            return await BuildPaymentStatusAsync(sessionId, userId);
        }

        public async Task HandleWebhookAsync(string json, string signatureHeader)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            if (string.IsNullOrWhiteSpace(webhookSecret))
                throw new InvalidOperationException("Stripe webhook secret is not configured.");

            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, webhookSecret);

            if (stripeEvent.Type != "checkout.session.completed")
                return;

            var session = stripeEvent.Data.Object as Session
                ?? throw new InvalidOperationException("Invalid Stripe checkout session payload.");

            await MarkOrderPaidFromSessionAsync(session, userId: null);
        }

        private async Task MarkOrderPaidFromSessionAsync(Session session, Guid? userId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.ProviderSessionId == session.Id);

            if (payment is null && session.Metadata.TryGetValue("orderId", out var orderIdValue)
                && Guid.TryParse(orderIdValue, out var orderId))
            {
                payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.OrderId == orderId);
            }

            if (payment is null)
                throw new InvalidOperationException("PAYMENT_NOT_FOUND");

            if (userId.HasValue && payment.UserId != userId.Value)
                throw new InvalidOperationException("PAYMENT_NOT_FOUND");

            if (payment.Status == PaymentStatus.Completed && payment.Order.Status == OrderStatus.Paid)
                return;

            payment.Status = PaymentStatus.Completed;
            payment.ProviderPaymentId = session.PaymentIntentId;
            payment.UpdatedAt = DateTime.UtcNow;

            payment.Order.Status = OrderStatus.Paid;
            payment.Order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notifications.NotifyRoleAsync("Admin", new NotificationMessage
            {
                Category = "payment",
                Severity = "success",
                Title = "Order paid",
                Message = $"Payment completed for order {payment.OrderId.ToString()[..8]}.",
                OrderId = payment.OrderId,
                Status = payment.Order.Status.ToString()
            });
        }

        private async Task<PaymentStatusResponse> BuildPaymentStatusAsync(string sessionId, Guid userId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.ProviderSessionId == sessionId && p.UserId == userId)
                ?? throw new InvalidOperationException("PAYMENT_NOT_FOUND");

            return new PaymentStatusResponse
            {
                OrderId = payment.OrderId,
                OrderStatus = payment.Order.Status.ToString(),
                PaymentStatus = payment.Status.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
            };
        }
    }
}
