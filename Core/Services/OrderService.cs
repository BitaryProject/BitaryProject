using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
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
    public class OrderService(IUnitOFWork unitOfWork ,
        IMapper mapper,
        IbasketRepository basketRepository)
        : IOrderService
    {
        public async Task<OrderResult?> CreateOrUptateOrderAsync(OrderRequest request, string userEmail)
        {

            var address = mapper.Map<Address>(request.ShippingAddress);

            var basket = await basketRepository.GetBasketAsync( request.BasketId)
                ?? throw new BasketNotFoundException( request.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Product, int>()
                    .GetAsync( item.Id) ?? throw new ProductNotFoundException( item.Id);
                orderItems.Add(CreateOrderItem(item,product));
            }

            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync( request.DeliveryMethodId)
                ?? throw new DeliverMethodNotFoundException( request.DeliveryMethodId);


            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var existingOrder = await orderRepo.GetAsync(new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId!));
            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
            }
            var subtotal = orderItems.Sum( item => item.Price * item.Quantity);
            var order = new Order(userEmail, shippingAddress: address, orderItems, deliveryMethod, subtotal ,basket.PaymentIntentId!);

            await orderRepo.AddAsync(order);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<OrderResult>( order);



        }

        private OrderItem CreateOrderItem(BasketItem item, Product product)
               => new OrderItem( new ProductInOrderItem( product.Id,product.Name,product.PictureUrl),
                     item.Quantity,
                     product.Price);


        public async Task<IEnumerable<DeliveryMethodResult?>> GetDeliveryMethodResult()
        {
            var methods = await unitOfWork.GetRepository<DeliveryMethod, int>()
                 .GetAllAsync();

            return mapper.Map<IEnumerable<DeliveryMethodResult>>(methods);
        }

        public async Task<OrderResult> GetOrderByIdAsync(Guid id)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>()
                .GetAsync( new OrderWithIncludeSpecifications(id))
                ?? throw new OrderNotFoundException(id);

            return mapper.Map<OrderResult>( order);
        }

        public async Task<IEnumerable<OrderResult>> GetOrderByEmailAsync(string email)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>()
                .GetAllAsync( new OrderWithIncludeSpecifications(email));

            return mapper.Map<IEnumerable<OrderResult>>( orders);
        }

      
    }
}
