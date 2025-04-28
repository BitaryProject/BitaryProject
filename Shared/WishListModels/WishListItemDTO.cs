using System;

namespace Shared.WishListModels
{
    public record WishListItemDTO
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public string ProductPictureUrl { get; init; }
        public decimal ProductPrice { get; init; }
        public string ProductBrand { get; init; }
        public string ProductCategory { get; init; }
        public DateTime AddedAt { get; init; }
    }
} 