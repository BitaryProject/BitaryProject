using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.ProductEntities
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int QuantityInStock { get; set; }
        
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
        
        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; }

        public int BrandId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public int CategoryId { get; set; }
    }
}

