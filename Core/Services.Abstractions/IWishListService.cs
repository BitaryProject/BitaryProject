using Shared.WishListModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IWishListService
    {
        Task<WishListDTO> GetUserWishListAsync(string userId);
        
        Task<WishListDTO> GetWishListByIdAsync(int id);
        
        Task<WishListItemDTO> AddItemToWishListAsync(string userId, int productId);
        
        Task<bool> RemoveItemFromWishListAsync(string userId, int itemId);
        
        Task<bool> RemoveProductFromWishListAsync(string userId, int productId);
        
        Task<bool> ClearWishListAsync(string userId);
        
        Task<bool> IsProductInWishListAsync(string userId, int productId);
    }
} 