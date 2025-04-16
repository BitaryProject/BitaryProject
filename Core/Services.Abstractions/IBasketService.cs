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
        public Task<(bool Success, CustomerBasketDTO? Basket)> UpdateBasketAsync(string basketId, BasketItemDTO itemDto);
        public Task<bool?> DeleteBasketAsync(string id);
        public Task<CustomerBasketDTO?> CreateBasketAsync();
        Task<CustomerBasketDTO?> UpdateItemQuantityAsync(Guid basketId, Guid itemId, UpdateBasketItemModel model);
        Task<CustomerBasketDTO?> RemoveItemAsync(Guid basketId, Guid itemId);
        Task<int> GetBasketItemCountAsync(Guid basketId);

        // Debug method to get detailed information about a basket
        Task<(bool BasketExists, int ItemCount, object BasketItems, object BasketDtoItems)> GetDebugBasketInfoAsync(Guid basketId);
    }
}
 