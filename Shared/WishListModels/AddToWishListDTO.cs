using System.ComponentModel.DataAnnotations;

namespace Shared.WishListModels
{
    public record AddToWishListDTO
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; init; }
    }
} 