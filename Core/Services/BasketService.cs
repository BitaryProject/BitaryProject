
global using Domain.Entities;
global using Domain.Exceptions;
global using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.BasketEntities;
using Shared.BasketModels;

namespace Services
{
    public class BasketServic(IbasketRepository basketRepository , IMapper mapper) : IBasketService
    {
        public async Task<bool?> DeleteBasketAsync(string id)

          => await basketRepository.DeleteBasketAsync(id);

      
        
                public async Task<CustomerBasketDTO?> GetBasketAsync(string id)
                {
                    var basket = await basketRepository.GetBasketAsync(id);
                    return basket is null ? throw new BasketNotFoundException(id)
                        : mapper.Map<CustomerBasketDTO>(basket);
                }
      
        

        public async Task<CustomerBasketDTO?> UpdateBasketAsync(BasketItemDTO basket)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var updateBasket =await basketRepository.UpdateBasketAsync(customerBasket);
            return updateBasket is null ?
                throw new Exception("can't update basket now!") :
                mapper.Map<CustomerBasketDTO>(updateBasket);

        }

        Task<CustomerBasketDTO?> IBasketService.DeleteBasketAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
