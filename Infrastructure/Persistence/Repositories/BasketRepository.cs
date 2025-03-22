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
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var existingBasket = await _context.CustomerBaskets
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.Id == basket.Id);

            if (existingBasket == null)
            {
                await _context.CustomerBaskets.AddAsync(basket);
            }
            else
            {
                _context.Entry(existingBasket).CurrentValues.SetValues(basket);
                _context.RemoveRange(existingBasket.BasketItems);
                existingBasket.BasketItems = basket.BasketItems;
            }

            var result = await _context.SaveChangesAsync();
            return result > 0 ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(Guid id)
        {
            var basket = await _context.CustomerBaskets.FirstOrDefaultAsync(b => b.Id == id);
            if (basket == null) return false;

            _context.CustomerBaskets.Remove(basket);
            return await _context.SaveChangesAsync() > 0;
        }

       
    }
}
