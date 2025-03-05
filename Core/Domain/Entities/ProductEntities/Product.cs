using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ProductEntities
{
    public class Product : BaseEntity<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PictureUrl { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public ProductBrand ProductBrand { get; set; }

        public int BrandId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public int CategoryId { get; set; }

    }
}
