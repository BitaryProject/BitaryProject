using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IOrderService
    {
        public Task<OrderResult?> GetOrderByIdAsync(Guid id);
        public Task<IEnumerable<OrderResult?>> GetOrderByEmailAsync(string email);
        public Task<OrderResult?> CreateOrUpdateOrderAsync(OrderRequest request ,string userEmail);
        public Task<IEnumerable<DeliveryMethodResult?>> GetDeliveryMethodResult();

    }
}

