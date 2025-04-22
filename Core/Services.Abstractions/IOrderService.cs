using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IOrderService
    {
        public Task<OrderResult?> GetOrderByIdAsync(Guid id);
        public Task<IEnumerable<OrderResult>> GetOrderByEmailAsync(string email);
        public Task<OrderResult?> CreateOrUpdateOrderAsync(OrderRequest request, string userEmail);
        public Task<IEnumerable<DeliveryMethodResult?>> GetDeliveryMethodResult();
        
        // Get all orders with optional filtering
        public Task<IEnumerable<OrderResult>> GetAllOrdersAsync(string? status = null, int pageNumber = 1, int pageSize = 10);
        
        // Update the email address for orders
        public Task<int> UpdateOrderEmailAsync(string sourceEmail, string targetEmail);
    }
}
