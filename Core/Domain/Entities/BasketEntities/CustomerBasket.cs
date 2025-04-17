using Core.Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.BasketEntities
{
    public class CustomerBasket : BaseEntity<Guid>
    {

        public CustomerBasket()
        {

        }

        public CustomerBasket(string clientSecret,
           decimal shippingPrice,
           ICollection<BasketItem> basketItems,
           string paymentIntentId
           )
        {
            Id = Guid.NewGuid();
            ClientSecret = clientSecret;
            ShippingPrice = shippingPrice;
            BasketItems = basketItems;
            PaymentIntentId = paymentIntentId;
        }
       
        public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        
        public decimal? ShippingPrice { get; set; }



    }
}

