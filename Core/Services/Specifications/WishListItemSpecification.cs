using Domain.Contracts;
using Domain.Entities.ProductEntities;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class WishListItemSpecification : Specifications<WishListItem>
    {
        // Get wishlist item by ID
        public WishListItemSpecification(int id)
            : base(wi => wi.Id == id)
        {
            AddInclude(wi => wi.Product);
            AddInclude(wi => wi.Product.ProductBrand);
            AddInclude(wi => wi.Product.ProductCategory);
        }
        
        // Check if product exists in wishlist
        public WishListItemSpecification(int wishListId, int productId)
            : base(wi => wi.WishListId == wishListId && wi.ProductId == productId)
        {
        }
        
        // Get wishlist items by wishlist ID
        public WishListItemSpecification(int wishListId, bool isWishListId = true)
            : base(wi => wi.WishListId == wishListId)
        {
            AddInclude(wi => wi.Product);
            AddInclude(wi => wi.Product.ProductBrand);
            AddInclude(wi => wi.Product.ProductCategory);
            setOrderByDescending(wi => wi.AddedAt);
        }
        
        // Get wishlist items using custom criteria
        public WishListItemSpecification(Expression<Func<WishListItem, bool>> criteria)
            : base(criteria)
        {
            AddInclude(wi => wi.Product);
            AddInclude(wi => wi.Product.ProductBrand);
            AddInclude(wi => wi.Product.ProductCategory);
        }
    }
} 