using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record BasketItemDTO
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public string PictureUrl { get; init; }
        public decimal Price { get; init; }

        public int Quantity { get; init; }
    }
}