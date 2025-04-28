using System;
using System.Collections.Generic;
using Domain.Entities.SecurityEntities;

namespace Domain.Entities.ProductEntities
{
    public class WishList : BaseEntity<int>
    {
        public WishList()
        {
            CreatedAt = DateTime.UtcNow;
            WishListItems = new HashSet<WishListItem>();
        }

        // Foreign key for User
        public string UserId { get; set; }
        
        // Navigation property for WishListItems
        public ICollection<WishListItem> WishListItems { get; set; }
        
        // Creation timestamp
        public DateTime CreatedAt { get; set; }
    }
} 