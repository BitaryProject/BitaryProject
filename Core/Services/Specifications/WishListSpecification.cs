using Domain.Contracts;
using Domain.Entities.ProductEntities;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class WishListSpecification : Specifications<WishList>
    {
        // Get wishlist by ID with items
        public WishListSpecification(int id)
            : base(w => w.Id == id)
        {
            AddInclude(w => w.WishListItems);
            AddInclude("WishListItems.Product");
            AddInclude("WishListItems.Product.ProductBrand");
            AddInclude("WishListItems.Product.ProductCategory");
        }
        
        // Get wishlist by user ID with items
        public WishListSpecification(string userId)
            : base(w => w.UserId == userId)
        {
            AddInclude(w => w.WishListItems);
            AddInclude("WishListItems.Product");
            AddInclude("WishListItems.Product.ProductBrand");
            AddInclude("WishListItems.Product.ProductCategory");
        }
        
        // Get wishlist with items using custom criteria
        public WishListSpecification(Expression<Func<WishList, bool>> criteria)
            : base(criteria)
        {
            AddInclude(w => w.WishListItems);
            AddInclude("WishListItems.Product");
            AddInclude("WishListItems.Product.ProductBrand");
            AddInclude("WishListItems.Product.ProductCategory");
        }
    }
} 