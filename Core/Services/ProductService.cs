global using Domain.Contracts;
global using Services.Abstractions;
global using Shared.ProductModels;
global using AutoMapper;
global using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Specifications;
using Shared;



namespace Services
{
    public class ProductService(IUnitOFWork UnitOFWork, IMapper Mapper) : IProductService
    {
        public async Task<IEnumerable<BrandResultDTO>> GetAllBrandsAsync()
        {
            var brands = await UnitOFWork.GetRepository<ProductBrand, int>().GetAllAsync();

            var brandsResult = Mapper.Map<IEnumerable<BrandResultDTO>>(brands);

            return brandsResult;
        }

        public async Task<PaginatedResult<ProductResultDTO>> GetAllProductsAsync(ProductSpecificationsParameters parameters)
        {
            var products = await UnitOFWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithBrandAndTypeSpecifications(parameters));

            var productsResult = Mapper.Map<IEnumerable<ProductResultDTO>>(products);

            var count = productsResult.Count();

            var totalCount = await UnitOFWork.GetRepository<Product, int>()
                .CountAsync(new ProductCountSpecifications(parameters));

            var result = new PaginatedResult<ProductResultDTO>(
                parameters.PageIndex,
               count,
                totalCount,
                productsResult);

            return result;
        }

        public async Task<IEnumerable<CategoryResultDTO>> GetAllCategoriesAsync()
        {
            var categories = await UnitOFWork.GetRepository<ProductCategory, int>().GetAllAsync();

            var categoriesResult = Mapper.Map<IEnumerable<CategoryResultDTO>>(categories);

            return categoriesResult;

        }

        public async Task<ProductResultDTO?> GetProductByIdAsync(int id)
        {
            var product = await UnitOFWork.GetRepository<Product, int>().GetAsync(
                new ProductWithBrandAndTypeSpecifications(id));

            return product is null ? throw new Exception($"Product with Id : {id} Not Found") : Mapper.Map<ProductResultDTO>(product);
        }
    }
}
