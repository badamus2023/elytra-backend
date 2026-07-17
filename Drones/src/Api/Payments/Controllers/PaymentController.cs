using Drones.src.Api.Auth.DTOs.Responses;
using Drones.src.Api.Payments.DTOs.Requests;
using Drones.src.Api.Payments.DTOs.Responses;
using Drones.src.Api.Payments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drones.src.Api.Payments.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("order/{orderId:guid}/checkout")]
        public async Task<ActionResult<CheckoutSessionResponse>> CreateCheckout(Guid orderId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService.CreateCheckoutSessionAsync(orderId, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("confirm")]
        public async Task<ActionResult<PaymentStatusResponse>> ConfirmPayment(
            ConfirmPaymentRequest request)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService.ConfirmCheckoutSessionAsync(
                    request.SessionId,
                    userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrWhiteSpace(signature))
                return BadRequest(new MessageResponse { Message = "Missing Stripe signature." });

            try
            {
                await _paymentService.HandleWebhookAsync(json, signature);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("User not authenticated.");

            return Guid.Parse(claim);
        }
    }
}
