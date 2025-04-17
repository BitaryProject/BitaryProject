global using Shared.ProductModels;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IProductService
    {
        public Task<PaginatedResult<ProductResultDTO>> GetAllProductsAsync(ProductSpecificationsParameters parameters);
        public Task<IEnumerable<BrandResultDTO>> GetAllBrandsAsync();
        public Task<IEnumerable<CategoryResultDTO>> GetAllCategoriesAsync();
        public Task<ProductResultDTO?> GetProductByIdAsync(int id);




    }
}

