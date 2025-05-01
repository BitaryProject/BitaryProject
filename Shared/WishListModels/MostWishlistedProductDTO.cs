using Shared.ProductModels;

namespace Shared.WishListModels
{
    public class MostWishlistedProductDTO
    {
        public ProductResultDTO Product { get; set; }
        public int WishlistCount { get; set; }
    }
} 