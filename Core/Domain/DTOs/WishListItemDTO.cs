using System;

namespace Domain.DTOs
{
    public class WishListItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPictureUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductBrand { get; set; }
        public string ProductCategory { get; set; }
        public DateTime AddedAt { get; set; }
    }
} 