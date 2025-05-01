using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Specifications
{
    public class ProductsWithBrandAndTypeByIdsSpecifications : Specifications<Product>
    {
        public ProductsWithBrandAndTypeByIdsSpecifications(IEnumerable<int> ids) 
            : base(product => ids.Contains(product.Id))
        {
            AddInclude(product => product.ProductBrand);
            AddInclude(product => product.ProductCategory);
        }
    }
} 