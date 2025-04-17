
using Domain.Entities.BasketEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IbasketRepository
    {
       public Task<CustomerBasket?> GetBasketAsync(Guid id);
        public Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
        public Task<bool> DeleteBasketAsync(Guid id);
        public Task<CustomerBasket?> CreateBasketAsync(CustomerBasket basket);


        // Add this debugging method
        public Task<int> GetBasketItemCountAsync(Guid basketId);
    }
}
