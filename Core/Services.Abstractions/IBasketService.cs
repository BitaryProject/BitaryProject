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
        public Task<CustomerBasketDTO?> UpdateBasketAsync(string basketId, BasketItemDTO itemDto);
        public Task<bool?> DeleteBasketAsync(string id);
    }
}
 