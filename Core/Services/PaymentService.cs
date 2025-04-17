global using Stripe;
global using Product = Domain.Entities.ProductEntities.Product;
using Domain.Contracts;
using Domain.Entities.OrderEntities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Core.Services.Specifications;
using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using Core.Domain.Contracts;
using AutoMapper;
using Core.Services.Abstractions;
using Core.Domain.Entities.OrderEntities;


namespace Services
{
    class PaymentService(IbasketRepository basketRepository,
     IUnitOFWork unitOfWork,
     IMapper mapper,
     IConfiguration configuration)
     : IPaymentService
    {
        public async Task<CustomerBasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = configuration.GetRequiredSection("StripeSettings")["SecretKey"];

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
            var endPointSecret = configuration.GetRequiredSection("StripeSettings")["EndPointSecret"];

            var stripeEvent = EventUtility.ConstructEvent(request, stripeHeader,
                    endPointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;


            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentPaymentFailed
                :
                    await UpdatePaymentStatusFailed(paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentSucceeded
                :
                    await UpdatePaymentStatusReceived(paymentIntent.Id);
                    break;
                default:
                    Console.WriteLine("UnHandled event type :{0}", stripeEvent.Type);
                    break;
            }
        }
        private async Task UpdatePaymentStatusFailed(string paymentIntentId)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new Exception();
            order.PaymentStatus = OrderPaymentStatus.PaymentFailed;
            unitOfWork.GetRepository<Order, Guid>().Update(order);

            await unitOfWork.SaveChangesAsync();


        }
        private async Task UpdatePaymentStatusReceived(string paymentIntentId)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new Exception();
            order.PaymentStatus = OrderPaymentStatus.PaymentReceived;
            unitOfWork.GetRepository<Order, Guid>().Update(order);

            await unitOfWork.SaveChangesAsync();


        }
    }

}
