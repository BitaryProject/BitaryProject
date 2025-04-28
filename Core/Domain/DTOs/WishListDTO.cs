using System;
using System.Collections.Generic;

namespace Domain.DTOs
{
    public class WishListDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WishListItemDTO> Items { get; set; } = new List<WishListItemDTO>();
    }
} 