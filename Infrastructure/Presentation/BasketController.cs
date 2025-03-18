using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BasketModels;

namespace Presentation
{
  
    public class BasketController(IServiceManager ServiceManager) : ApiController
    {
        [HttpGet("{id}" )]
        public async Task<ActionResult<CustomerBasketDTO>> Get(string id)
        {
            var basket = await ServiceManager.BasketService.GetBasketAsync(id);

            return Ok(basket);
        }


        [HttpPost]
        public async Task<ActionResult<CustomerBasketDTO>> Update(BasketItemDTO basketDTO)
        {
            var basket = await ServiceManager.BasketService.UpdateBasketAsync(basketDTO);

            return Ok(basket);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
           await ServiceManager.BasketService.DeleteBasketAsync(id);

            return NoContent();
        }
    }
}
