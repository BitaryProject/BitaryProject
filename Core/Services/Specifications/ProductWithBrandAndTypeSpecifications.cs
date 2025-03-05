using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class ProductWithBrandAndTypeSpecifications : Specifications<Product>
    {
        public ProductWithBrandAndTypeSpecifications(int id) : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductBrand);
            AddInclude(product => product.ProductCategory);
        }
        public ProductWithBrandAndTypeSpecifications(ProductSpecificationsParameters parameters)
            : base(product =>
            (!parameters.BrandId.HasValue || product.BrandId == parameters.BrandId.Value) &&
            (!parameters.CategoryId.HasValue || product.CategoryId == parameters.CategoryId.Value) &&
            (string.IsNullOrWhiteSpace(parameters.Search) || product.Name.ToLower().Contains(parameters.Search.ToLower().Trim())))
        {
            AddInclude(product => product.ProductBrand);
            AddInclude(product => product.ProductCategory);

            ApplyPagination(parameters.PageIndex, parameters.PageSize);

            if (parameters.Sort is not null)
            {
                switch (parameters.Sort)
                {
                    case ProductSortingOptions.NameDesc:
                        setOrderByDescending(product => product.Name);
                        break;
                    case ProductSortingOptions.NameAsc:
                        setOrderBy(product => product.Name);
                        break;

                    case ProductSortingOptions.PriceDesc:
                        setOrderByDescending(product => product.Price);
                        break;
                    case ProductSortingOptions.PriceAsc:
                        setOrderBy(product => product.Price);
                        break;

                    default:
                        break;

                }
            }
        }

    }
}
