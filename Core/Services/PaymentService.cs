global using Stripe;
global using Product = Domain.Entities.ProductEntities.Product;
global using PaymentMethodEnum = Shared.OrderModels.PaymentMethod;

using Domain.Contracts;
using Domain.Entities.OrderEntities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Services.Specifications;
using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class PaymentService(IbasketRepository basketRepository,
     IUnitOFWork unitOfWork,
     IMapper mapper,
     IConfiguration configuration)
     : IPaymentService
    {
        private readonly string _stripeSecretKey = configuration.GetRequiredSection("StripeSettings")["SecretKey"];
        private readonly string _stripeEndpointSecret = configuration.GetRequiredSection("StripeSettings")["EndPointSecret"];

        #region Legacy Methods

        public async Task<CustomerBasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = _stripeSecretKey;

            // Get Basket ⇒ SubTotal ⇒ Product
            if (!Guid.TryParse(basketId, out var basketGuid))
                throw new Exception($"Invalid basket Id format: {basketId}");

            var basket = await basketRepository.GetBasketAsync(basketGuid)
                ?? throw new BasketNotFoundException(basketId);
           

            foreach (var item in basket.BasketItems)
            {
                var product = await unitOfWork.GetRepository<Product, int>()
                    .GetAsync(item.Product.ProductId) ?? throw new ProductNotFoundException(item.Product.ProductId);

                item.Price = product.Price;
            }

            if (!basket.DeliveryMethodId.HasValue)
                throw new Exception("No Delivery Method Is Selected");

            var method = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync(basket.DeliveryMethodId.Value)
                ?? throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShippingPrice = method.Price;

            var amount = (long)(basket.BasketItems.Sum(item => item.Quantity * item.Price) + basket.ShippingPrice) * 100;

            var service = new PaymentIntentService();

            if (string.IsNullOrWhiteSpace(basket.PaymentIntentId))
            {
                // Create
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var paymentIntent = await service.CreateAsync(createOptions);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };

                await service.UpdateAsync(basket.PaymentIntentId, updateOptions);
            }

            await basketRepository.UpdateBasketAsync(basket);
            return mapper.Map<CustomerBasketDTO>(basket);
        }

        public async Task UpdateOrderPaymentStatus(string request, string stripeHeader)
        {
            var stripeEvent = EventUtility.ConstructEvent(request, stripeHeader, _stripeEndpointSecret);
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            Console.WriteLine($"Received Stripe webhook: {stripeEvent.Type}");

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentPaymentFailed:
                    await UpdatePaymentStatusFailed(paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentSucceeded:
                    await UpdatePaymentStatusReceived(paymentIntent.Id);
                    break;
                default:
                    Console.WriteLine($"Unhandled Stripe event type: {stripeEvent.Type}");
                    break;
            }
        }

        private async Task UpdatePaymentStatusFailed(string paymentIntentId)
        {
            Console.WriteLine($"Updating payment status to Failed for PaymentIntentId: {paymentIntentId}");
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new OrderNotFoundException(Guid.Empty, $"Order with PaymentIntentId {paymentIntentId} not found");
                
            order.PaymentStatus = OrderPaymentStatus.PaymentFailed;
            unitOfWork.GetRepository<Order, Guid>().Update(order);
            await unitOfWork.SaveChangesAsync();
            Console.WriteLine($"Payment status updated to Failed for Order: {order.Id}");
        }

        private async Task UpdatePaymentStatusReceived(string paymentIntentId)
        {
            Console.WriteLine($"Updating payment status to Received for PaymentIntentId: {paymentIntentId}");
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new OrderNotFoundException(Guid.Empty, $"Order with PaymentIntentId {paymentIntentId} not found");
                
            order.PaymentStatus = OrderPaymentStatus.PaymentReceived;
            unitOfWork.GetRepository<Order, Guid>().Update(order);
            await unitOfWork.SaveChangesAsync();
            Console.WriteLine($"Payment status updated to Received for Order: {order.Id}");
        }

        #endregion

        #region New Payment Methods

        public async Task<OrderResult> ProcessPaymentAsync(PaymentRequestDTO paymentRequest, string userEmail)
        {
            Console.WriteLine($"Processing payment for order {paymentRequest.OrderId} with method {paymentRequest.PaymentMethod}");
            
            // Validate the order exists and belongs to the user
            var order = await GetOrderByIdAndValidateOwnership(paymentRequest.OrderId, userEmail);
            
            // Process payment based on selected payment method
            switch (paymentRequest.PaymentMethod)
            {
                case PaymentMethodEnum.Stripe:
                    return await CreateStripePaymentIntentAsync(order.Id);
                    
                case PaymentMethodEnum.Cash:
                    return await ProcessCashPaymentAsync(order.Id);
                    
                default:
                    throw new ArgumentException($"Unsupported payment method: {paymentRequest.PaymentMethod}");
            }
        }

        public async Task<OrderResult> CreateStripePaymentIntentAsync(Guid orderId)
        {
            Console.WriteLine($"Creating Stripe payment intent for order {orderId}");
            StripeConfiguration.ApiKey = _stripeSecretKey;
            
            // Get the order with all related data
            var order = await GetOrderWithDetails(orderId);
            
            // Calculate the total amount (in cents for Stripe)
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync(order.DeliveryMethodId ?? 0);
                
            decimal deliveryPrice = deliveryMethod?.Price ?? 0;
            var amount = (long)((order.Subtotal + deliveryPrice) * 100);
            
            Console.WriteLine($"Payment amount: {amount / 100.0m:C} (Subtotal: {order.Subtotal:C}, Delivery: {deliveryPrice:C})");
            
            try
            {
                // Create a new payment intent using Stripe API
                var service = new PaymentIntentService();
                PaymentIntent intent;
                
                // Check if the payment intent ID is valid - it should not be null, empty, "NoPaymentYet" or start with "default_"
                bool isValidPaymentIntentId = !string.IsNullOrEmpty(order.PaymentIntentId) && 
                                             order.PaymentIntentId != "NoPaymentYet" && 
                                             !order.PaymentIntentId.StartsWith("default_");
                
                if (!isValidPaymentIntentId)
                {
                    // Create a new payment intent
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = amount,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" },
                        Metadata = new Dictionary<string, string>
                        {
                            { "OrderId", order.Id.ToString() }
                        }
                    };
                    
                    intent = await service.CreateAsync(options);
                    
                    // Update the order with the payment intent ID
                    order.PaymentIntentId = intent.Id;
                    Console.WriteLine($"Created new payment intent: {intent.Id}");
                }
                else
                {
                    try {
                        // Try to update existing payment intent
                        var options = new PaymentIntentUpdateOptions
                        {
                            Amount = amount
                        };
                        
                        intent = await service.UpdateAsync(order.PaymentIntentId, options);
                        Console.WriteLine($"Updated existing payment intent: {intent.Id}");
                    }
                    catch (StripeException ex) when (ex.Message.Contains("No such payment_intent"))
                    {
                        // If the payment intent doesn't exist in Stripe, create a new one
                        Console.WriteLine($"Payment intent {order.PaymentIntentId} not found in Stripe, creating a new one");
                        var options = new PaymentIntentCreateOptions
                        {
                            Amount = amount,
                            Currency = "usd",
                            PaymentMethodTypes = new List<string> { "card" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "OrderId", order.Id.ToString() }
                            }
                        };
                        
                        intent = await service.CreateAsync(options);
                        
                        // Update the order with the new payment intent ID
                        order.PaymentIntentId = intent.Id;
                        Console.WriteLine($"Created new payment intent: {intent.Id}");
                    }
                }
                
                // Update order status and save changes
                order.PaymentStatus = OrderPaymentStatus.Pending;
                unitOfWork.GetRepository<Order, Guid>().Update(order);
                await unitOfWork.SaveChangesAsync();
                
                // Map order to result with client secret
                var result = mapper.Map<OrderResult>(order);
                result.PaymentIntentId = intent.Id;
                result.ClientSecret = intent.ClientSecret;
                
                return result;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe error: {ex.Message}");
                throw new Exception($"Error creating payment intent: {ex.Message}", ex);
            }
        }

        public async Task<OrderResult> ProcessCashPaymentAsync(Guid orderId)
        {
            Console.WriteLine($"Processing cash payment for order {orderId}");
            
            // Get the order
            var order = await GetOrderWithDetails(orderId);
            
            // Update order status to pending payment (awaiting cash receipt)
            order.PaymentStatus = OrderPaymentStatus.Pending;
            order.PaymentIntentId = $"CASH_{Guid.NewGuid()}"; // Generate a unique reference for cash payment
            
            // Save changes
            unitOfWork.GetRepository<Order, Guid>().Update(order);
            await unitOfWork.SaveChangesAsync();
            
            Console.WriteLine($"Cash payment marked as pending for order {orderId}");
            
            // Return updated order
            return mapper.Map<OrderResult>(order);
        }

        public async Task<OrderResult> UpdateOrderPaymentStatusAsync(Guid orderId, OrderPaymentStatus status)
        {
            Console.WriteLine($"Admin updating payment status for order {orderId} to {status}");
            
            // Get the order
            var order = await GetOrderWithDetails(orderId);
            
            // Update payment status
            order.PaymentStatus = status;
            
            // If payment is received, record the date
            if (status == OrderPaymentStatus.PaymentReceived)
            {
                // You might want to add a PaymentDate property to your Order class
                // order.PaymentDate = DateTimeOffset.Now;
            }
            
            // Save changes
            unitOfWork.GetRepository<Order, Guid>().Update(order);
            await unitOfWork.SaveChangesAsync();
            
            Console.WriteLine($"Payment status updated to {status} for order {orderId}");
            
            // Return updated order
            return mapper.Map<OrderResult>(order);
        }

        public async Task<OrderResult> GetPaymentDetailsByOrderIdAsync(Guid orderId)
        {
            Console.WriteLine($"Getting payment details for order {orderId}");
            
            // Get the order with all related data
            var order = await GetOrderWithDetails(orderId);
            
            // Map to order result
            var result = mapper.Map<OrderResult>(order);
            
            // If it's a Stripe payment and has a payment intent ID, get current status from Stripe
            if (!string.IsNullOrEmpty(order.PaymentIntentId) && !order.PaymentIntentId.StartsWith("CASH_"))
            {
                try
                {
                    StripeConfiguration.ApiKey = _stripeSecretKey;
                    var service = new PaymentIntentService();
                    var intent = await service.GetAsync(order.PaymentIntentId);
                    
                    // Add additional payment details to the result
                    result.StripeStatus = intent.Status;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving Stripe payment intent: {ex.Message}");
                    // Continue and return the order without Stripe details
                }
            }
            
            return result;
        }

        #endregion

        #region Helper Methods

        private async Task<Order> GetOrderWithDetails(Guid orderId)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>()
                .GetAsync(new OrderWithIncludeSpecifications(orderId))
                ?? throw new OrderNotFoundException(orderId);
                
            return order;
        }
        
        private async Task<Order> GetOrderByIdAndValidateOwnership(Guid orderId, string userEmail)
        {
            var order = await GetOrderWithDetails(orderId);
            
            // Validate ownership if email is provided (skip for admin operations)
            if (!string.IsNullOrEmpty(userEmail) && order.UserEmail != userEmail)
            {
                Console.WriteLine($"Order ownership validation failed. Order email: {order.UserEmail}, User email: {userEmail}");
                throw new UnauthorizedAccessException("You don't have permission to access this order");
            }
            
            return order;
        }

        #endregion
    }
}
