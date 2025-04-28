using System;

namespace Domain.Entities.ProductEntities
{
    public class WishListItem : BaseEntity<int>
    {
        public WishListItem()
        {
            AddedAt = DateTime.UtcNow;
        }

        // Foreign key for WishList
        public int WishListId { get; set; }
        
        // Foreign key for Product
        public int ProductId { get; set; }
        
        // Navigation properties
        public WishList WishList { get; set; }
        public Product Product { get; set; }
        
        // Timestamp when item was added to wishlist
        public DateTime AddedAt { get; set; }
    }
} 