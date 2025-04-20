using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IBasketService
    {
        public Task<CustomerBasketDTO?> GetBasketAsync(string id);
        public Task<bool> UpdateBasketAsync(string basketId, BasketItemDTO itemDto);
        public Task<bool> AddItemToBasketAsync(string basketId, int productId, int quantity);
        public Task<bool> DeleteBasketAsync(string id);
        public Task<(bool Success, string BasketId)> CreateBasketAsync();
        Task<bool> UpdateItemQuantityAsync(string basketId, int productId, int quantity);
        Task<bool> RemoveItemAsync(string basketId, int productId);
        Task<int> GetBasketItemCountAsync(Guid basketId);
        
        // New method for updating delivery method
        Task<bool> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId);

        // Debug method to get detailed information about a basket
        Task<(bool BasketExists, int ItemCount, object BasketItems, object BasketDtoItems)> GetDebugBasketInfoAsync(Guid basketId);
    }
}
 