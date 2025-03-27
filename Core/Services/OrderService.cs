using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
using Services.Specifications;
using OrderAddress = Domain.Entities.OrderEntities.Address;

using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;
using Address = Domain.Entities.OrderEntities.Address;



namespace Services
{
    public class OrderService(IUnitOFWork unitOfWork ,
        IMapper mapper,
        IbasketRepository basketRepository)
        : IOrderService
    {
        public async Task<OrderResult?> CreateOrUpdateOrderAsync(OrderRequest request, string userEmail)
        {
           
            var address = mapper.Map<Address>(request.ShippingAddress);

           /* if (!Guid.TryParse(request.BasketId, out var basketGuid))
            {
                throw new Exception($"Invalid basket Id format: {request.BasketId}");
            }*/
            var basket = await basketRepository.GetBasketAsync(Guid.Parse(request.BasketId ))
                ?? throw new BasketNotFoundException( request.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.BasketItems)
            {
                var product = await unitOfWork.GetRepository<Product, int>()
                    .GetAsync( item.Product.ProductId) ?? throw new ProductNotFoundException( item.Product.ProductId);
                orderItems.Add(CreateOrderItem(item,product));
            }

            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync( request.DeliveryMethodId)
                ?? throw new DeliveryMethodNotFoundException( request.DeliveryMethodId);


            var orderRepo = unitOfWork.GetRepository<Order, Guid>();

            var existingOrder = await orderRepo.
                GetAsync(new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId));

            if (existingOrder is not null)
                orderRepo.Delete(existingOrder);


            // subtotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            // save to DB
            var order = new Order(userEmail, address, orderItems, deliveryMethod, subTotal, basket.PaymentIntentId);
            await unitOfWork.GetRepository<Order, Guid>()
                .AddAsync(order);
           // await orderRepo.AddAsync(order);


            await unitOfWork.SaveChangesAsync();
            // Map and Return

            return mapper.Map<OrderResult>(order);



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

        //GetAllOrders with filteration in db
        public async Task<IEnumerable<OrderResult>> GetAllOrdersAsync(string? status = null, int pageNumber = 1, int pageSize = 10)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();

            var ordersQuery = orderRepo.GetAllAsQueryable(); 

            if (!string.IsNullOrEmpty(status))
                ordersQuery = ordersQuery.Where(o => o.PaymentStatus.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

            var paginatedOrders = ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var orders = await paginatedOrders.ToListAsync(); 

            return mapper.Map<IEnumerable<OrderResult>>(orders);
        }



    }
}
