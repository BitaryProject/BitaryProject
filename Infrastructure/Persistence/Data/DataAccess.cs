using Domain.Entities.ProductEntities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly StoreContext _dbContext;

        public DataAccess(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WishList> GetWishListByUserIdAsync(string userId)
        {
            return await _dbContext.WishLists
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.ProductBrand)
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.ProductCategory)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<WishList> GetWishListByIdAsync(int id)
        {
            return await _dbContext.WishLists
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.ProductBrand)
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.ProductCategory)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WishListItem> FindWishListItemAsync(int id)
        {
            return await _dbContext.WishListItems.FindAsync(id);
        }

        public async Task<WishListItem> GetWishListItemAsync(int wishListId, int productId)
        {
            return await _dbContext.WishListItems
                .FirstOrDefaultAsync(i => i.WishListId == wishListId && i.ProductId == productId);
        }

        public async Task<List<WishListItem>> GetWishListItemsByWishListIdAsync(int wishListId)
        {
            return await _dbContext.WishListItems
                .Where(item => item.WishListId == wishListId)
                .ToListAsync();
        }

        public async Task<bool> IsProductInWishListAsync(string userId, int productId)
        {
            return await _dbContext.WishLists
                .Where(w => w.UserId == userId)
                .SelectMany(w => w.WishListItems)
                .AnyAsync(i => i.ProductId == productId);
        }

        public async Task AddWishListAsync(WishList wishList)
        {
            await _dbContext.WishLists.AddAsync(wishList);
        }

        public async Task AddWishListItemAsync(WishListItem item)
        {
            await _dbContext.WishListItems.AddAsync(item);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveWishListItemAsync(WishListItem item)
        {
            _dbContext.WishListItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveWishListItemsAsync(IEnumerable<WishListItem> items)
        {
            _dbContext.WishListItems.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }
    }
} 