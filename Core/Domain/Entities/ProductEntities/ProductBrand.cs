using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Entities.ProductEntities
{
    public class ProductBrand : BaseEntity<int>
    {
        public string Name { get; set; }
        
        public ICollection<Product> Products { get; set; }
    }
}

