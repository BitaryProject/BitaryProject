using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public record BasketItemDTO
    {
        public int Id { get; init; }
        public string ProductName { get; init; }
        public string PictureUrl { get; init; }
        public decimal Price { get; init; }
        public string Category { get; init; }
        public string Brand { get; init; }
        public int Quantity { get; init; }
    }
}