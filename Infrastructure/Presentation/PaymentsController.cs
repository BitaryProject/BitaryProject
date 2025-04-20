using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public PaymentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        #region Legacy Endpoints

        [HttpPost("basket/{basketId}")]
        [Authorize]
        public async Task<ActionResult<CustomerBasketDTO>> CreateOrUpdatePaymentIntent(string basketId)
        {
            Console.WriteLine($"Creating/updating payment intent for basket {basketId}");
            try
            {
                var result = await _serviceManager.PaymentService.CreateOrUpdatePaymentIntentAsync(basketId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating payment intent: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signatureHeader = Request.Headers["Stripe-Signature"].ToString();

            try
            {
                await _serviceManager.PaymentService.UpdateOrderPaymentStatus(json, signatureHeader);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing webhook: {ex.Message}");
                return BadRequest();
            }
        }

        #endregion

        #region New Payment Endpoints

        [HttpPost("process")]
        [Authorize]
        public async Task<ActionResult<OrderResult>> ProcessPayment([FromBody] PaymentRequestDTO paymentRequest)
        {
            Console.WriteLine($"Processing payment for order {paymentRequest.OrderId}");
            try
            {
                // Get the current user's email
                var email = User.FindFirstValue(ClaimTypes.Email);
                
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Warning: User email not found in token");
                    return Unauthorized(new { error = "User email not found in token" });
                }
                
                Console.WriteLine($"User email from token: {email}");
                
                var result = await _serviceManager.PaymentService.ProcessPaymentAsync(paymentRequest, email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized: {ex.Message}");
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("stripe/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderResult>> CreateStripePaymentIntent(Guid orderId)
        {
            Console.WriteLine($"Creating Stripe payment intent for order {orderId}");
            try
            {
                var result = await _serviceManager.PaymentService.CreateStripePaymentIntentAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Stripe payment intent: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("cash/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderResult>> ProcessCashPayment(Guid orderId)
        {
            Console.WriteLine($"Processing cash payment for order {orderId}");
            try
            {
                var result = await _serviceManager.PaymentService.ProcessCashPaymentAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing cash payment: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("order/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderResult>> GetPaymentDetails(Guid orderId)
        {
            Console.WriteLine($"Getting payment details for order {orderId}");
            try
            {
                var result = await _serviceManager.PaymentService.GetPaymentDetailsByOrderIdAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting payment details: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Admin Endpoints

        [HttpPut("admin/status")]
        [Authorize(Roles = "Admin")] // Ensure only admins can access this endpoint
        public async Task<ActionResult<OrderResult>> UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            Console.WriteLine($"Admin updating payment status for order {request.OrderId} to {request.Status}");
            try
            {
                var result = await _serviceManager.PaymentService.UpdateOrderPaymentStatusAsync(request.OrderId, request.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating payment status: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
