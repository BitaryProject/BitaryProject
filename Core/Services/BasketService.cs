using Domain.Entities.BasketEntites;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BasketServic(IbasketRepository basketRepository , IMapper mapper) : IBasketService
    {
        public async Task<bool?> DeleteBasketAsync(string id)

          => await basketRepository.DeleteBasketAsync(id);

      
        
                public async Task<BasketDTO?> GetBasketAsync(string id)
                {
                    var basket = await basketRepository.GetBasketAsync(id);
                    return basket is null ? throw new BasketNotFoundException(id)
                        : mapper.Map<BasketDTO>(basket);
                }
      
        

        public async Task<BasketDTO?> UpdateBasketAsync(BasketItemDTO basket)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var updateBasket =await basketRepository.UpdateBasketAsync(customerBasket);
            return updateBasket is null ?
                throw new Exception("can't update basket now!") :
                mapper.Map<BasketDTO>(updateBasket);

        }

        Task<BasketDTO?> IBasketService.DeleteBasketAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
