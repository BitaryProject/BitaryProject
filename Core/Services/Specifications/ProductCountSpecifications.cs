using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Entities.ProductEntities;
using Shared.ProductModels;

namespace Core.Services.Specifications
{
    public class ProductCountSpecifications : Specifications<Product>
    {
        public ProductCountSpecifications(ProductSpecificationsParameters parameters)
            : base(product =>
            (!parameters.BrandId.HasValue || product.BrandId == parameters.BrandId.Value) &&
            (!parameters.CategoryId.HasValue || product.CategoryId == parameters.CategoryId.Value) &&
            (string.IsNullOrWhiteSpace(parameters.Search) || product.Name.ToLower().Contains(parameters.Search.ToLower().Trim())))
        {



        }
    }
}
