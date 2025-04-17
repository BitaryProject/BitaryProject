using Core.Domain.Entities.OrderEntities;
using Core.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.BasketEntities
{
    public class BasketItem : BaseEntity<Guid>
    {

        public BasketItem()
        {

        }
        public BasketItem(ProductInCartItem product, int quantity, decimal price)
        {
            Product = product;
            Quantity = quantity;
            Price = price;
        }




       //public int Id { get; set; }

        public ProductInCartItem Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

