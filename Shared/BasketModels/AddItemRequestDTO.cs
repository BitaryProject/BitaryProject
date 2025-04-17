using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record AddItemRequestDTO
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
} 