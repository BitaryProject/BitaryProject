using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record CustomerBasketDTO
    {
        public string Id { get; init; }
        public IEnumerable<BasketItemDTO> Items { get; init; } = Enumerable.Empty<BasketItemDTO>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal? ShippingPrice { get; set; }

    }
}
