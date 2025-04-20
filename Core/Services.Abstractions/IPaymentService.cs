using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;
using Shared;
using Shared.BasketModels;
using Shared.OrderModels;

namespace Services.Abstractions
{
    public interface IPaymentService
    {
        // Legacy method - will be deprecated
        public Task<CustomerBasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);
        
        // Stripe webhook handler
        public Task UpdateOrderPaymentStatus(string request, string stripeHeader);
        
        // New methods for multiple payment types
        public Task<OrderResult> ProcessPaymentAsync(PaymentRequestDTO paymentRequest, string userEmail);
        
        // Create Stripe payment intent for an order
        public Task<OrderResult> CreateStripePaymentIntentAsync(Guid orderId);
        
        // Process cash payment (for COD orders)
        public Task<OrderResult> ProcessCashPaymentAsync(Guid orderId);
        
        // Admin method to update order payment status
        public Task<OrderResult> UpdateOrderPaymentStatusAsync(Guid orderId, OrderPaymentStatus status);
        
        // Get payment details for an order
        public Task<OrderResult> GetPaymentDetailsByOrderIdAsync(Guid orderId);
    }
}
