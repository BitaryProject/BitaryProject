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


        [HttpPost("{basketId}/items")]
        public async Task<ActionResult<CustomerBasketDTO>> AddItem(string basketId, [FromBody] BasketItemDTO itemDto)
        {
            var basket = await ServiceManager.BasketService.UpdateBasketAsync(basketId, itemDto);
            return Ok(basket);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
           await ServiceManager.BasketService.DeleteBasketAsync(id);

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<CustomerBasketDTO>> CreateBasket()
        {           
            var basketDto = await ServiceManager.BasketService.CreateBasketAsync();
            return Ok(basketDto);
        }

        [HttpPut("{basketId}/items/{itemId}")]
        public async Task<ActionResult<CustomerBasketDTO>> UpdateItemQuantity(Guid basketId, Guid itemId, [FromBody] UpdateBasketItemModel model)
        {
            if (model == null)
                return BadRequest("No data provided.");

            var updatedBasket = await ServiceManager.BasketService.UpdateItemQuantityAsync(basketId, itemId, model);
            if (updatedBasket == null)
                return NotFound("Basket or item not found.");

            return Ok(updatedBasket);
        }


        [HttpDelete("{basketId}/items/{itemId}")]
        public async Task<ActionResult<CustomerBasketDTO>> RemoveItem(Guid basketId, Guid itemId)
        {
            var updatedBasket = await ServiceManager.BasketService.RemoveItemAsync(basketId, itemId);
            if (updatedBasket == null)
                return NotFound("Basket or item not found.");

            return Ok(updatedBasket);
        }
    }
}
