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
        public Task<BasketDTO?> GetBasketAsync(string id);
        public Task<BasketDTO?> UpdateBasketAsync(BasketItemDTO basket);
        public Task<BasketDTO?> DeleteBasketAsync(string id);
    }
}
 