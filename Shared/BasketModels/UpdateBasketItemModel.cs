using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record UpdateBasketItemModel
    {
        public int Quantity { get; set; }

    }
}
