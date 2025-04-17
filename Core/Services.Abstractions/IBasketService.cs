using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IBasketService
    {
        public Task<CustomerBasketDTO?> GetBasketAsync(string id);
        public Task<CustomerBasketDTO?> UpdateBasketAsync(string basketId, BasketItemDTO itemDto);
        public Task<bool?> DeleteBasketAsync(string id);
        public Task<CustomerBasketDTO?> CreateBasketAsync();
        Task<CustomerBasketDTO?> UpdateItemQuantityAsync(Guid basketId, Guid itemId, UpdateBasketItemModel model);
        Task<CustomerBasketDTO?> RemoveItemAsync(Guid basketId, Guid itemId);

    }
}
 
