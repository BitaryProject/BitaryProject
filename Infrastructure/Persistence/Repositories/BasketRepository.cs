using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class BasketRepository : IbasketRepository
    {
        private readonly StoreContext _context;

        public BasketRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<CustomerBasket?> GetBasketAsync(Guid id)
        {
            return await _context.CustomerBaskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var existingBasket = await _context.CustomerBaskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.Id == basket.Id);

            if (existingBasket == null)
            {
                await _context.CustomerBaskets.AddAsync(basket);
            }
            else
            {
                // Update scalar properties
                _context.Entry(existingBasket).CurrentValues.SetValues(basket);

                // Handle items
                foreach (var newItem in basket.BasketItems)
                {
                    var existingItem = existingBasket.BasketItems
                        .FirstOrDefault(i => i.Product.ProductId == newItem.Product.ProductId);

                    if (existingItem != null)
                    {
                        // Update existing item
                        _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                        existingItem.Product = newItem.Product;
                    }
                    else
                    {
                        existingBasket.BasketItems.Add(newItem);
                    }
                }

                // Remove deleted items
                foreach (var existingItem in existingBasket.BasketItems.ToList())
                {
                    if (!basket.BasketItems.Any(i => i.Product.ProductId == existingItem.Product.ProductId))
                    {
                        existingBasket.BasketItems.Remove(existingItem);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return existingBasket ?? basket;
        }

        public async Task<bool> DeleteBasketAsync(Guid id)
        {
            var basket = await _context.CustomerBaskets.FirstOrDefaultAsync(b => b.Id == id);
            if (basket == null) return false;

            _context.CustomerBaskets.Remove(basket);
            return await _context.SaveChangesAsync() > 0;
        }





        public async Task<CustomerBasket?> CreateBasketAsync(CustomerBasket basket)
        {
            await _context.CustomerBaskets.AddAsync(basket);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? basket : null;
        }



        public async Task<bool> RemoveItemAsync(Guid basketId, Guid itemId)
        {
            var basket = await _context.CustomerBaskets
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.Id == basketId);

            if (basket == null)
                return false;

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return false;

            basket.BasketItems.Remove(item);

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }

}
