using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.BasketEntities
{
    [Owned]
    public class ProductInCartItem
    {

        public ProductInCartItem()
        {

        }
        public ProductInCartItem(int productId, string productName, string pictureUrl)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
    }
}


