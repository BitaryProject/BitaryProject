using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDataAccess
    {
        Task<WishList> GetWishListByUserIdAsync(string userId);
        Task<WishList> GetWishListByIdAsync(int id);
        Task<WishListItem> FindWishListItemAsync(int id);
        Task<WishListItem> GetWishListItemAsync(int wishListId, int productId);
        Task<List<WishListItem>> GetWishListItemsByWishListIdAsync(int wishListId);
        Task<bool> IsProductInWishListAsync(string userId, int productId);
        Task AddWishListAsync(WishList wishList);
        Task AddWishListItemAsync(WishListItem item);
        Task SaveChangesAsync();
        Task RemoveWishListItemAsync(WishListItem item);
        Task RemoveWishListItemsAsync(IEnumerable<WishListItem> items);
    }
} 