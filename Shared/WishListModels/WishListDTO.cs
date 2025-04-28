using System;
using System.Collections.Generic;

namespace Shared.WishListModels
{
    public record WishListDTO
    {
        public int Id { get; init; }
        public string UserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<WishListItemDTO> Items { get; init; } = new List<WishListItemDTO>();
    }
} 