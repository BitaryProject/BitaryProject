using Domain.Contracts;
using Core.Services.Abstractions;
using Shared.ProductModels;
using AutoMapper;
using Core.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Services.Specifications;
using Shared;
using Domain.Exceptions;
using Core.Common.Specifications;
using Core.Services.Specifications.Base;

namespace Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BrandResultDTO>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand>().GetAllAsync();
            var brandsResult = _mapper.Map<IEnumerable<BrandResultDTO>>(brands);
            return brandsResult;
        }

        public async Task<PaginatedResult<ProductResultDTO>> GetAllProductsAsync(ProductSpecificationsParameters parameters)
        {
            // Create a BaseSpecification to replace ProductWithBrandAndTypeSpecifications
            var spec = new BaseSpecification<Product>(p => true);
            
            // Apply sorting and filtering based on parameters
            if (!string.IsNullOrEmpty(parameters.Sort))
            {
                if (parameters.Sort.Equals("priceAsc", StringComparison.OrdinalIgnoreCase))
                    spec.ApplyOrderBy(p => p.Price);
                else if (parameters.Sort.Equals("priceDesc", StringComparison.OrdinalIgnoreCase))
                    spec.ApplyOrderByDescending(p => p.Price);
                else
                    spec.ApplyOrderBy(p => p.Name);
            }
            
            // Apply paging
            spec.ApplyPaging((parameters.PageIndex - 1) * parameters.PageSize, parameters.PageSize);
            
            // Add includes
            spec.AddInclude(p => p.Brand);
            spec.AddInclude(p => p.Category);

            var products = await _unitOfWork.GetRepository<Product>().GetAllAsync(spec);
            var productsResult = _mapper.Map<IEnumerable<ProductResultDTO>>(products);
            var count = productsResult.Count();

            // Create count specification
            var countSpec = new BaseSpecification<Product>(p => true);
            var totalCount = await _unitOfWork.GetRepository<Product>().CountAsync(countSpec);

            var result = new PaginatedResult<ProductResultDTO>(
                parameters.PageIndex,
                count,
                totalCount,
                productsResult);

            return result;
        }

        public async Task<IEnumerable<CategoryResultDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.GetRepository<ProductCategory>().GetAllAsync();
            var categoriesResult = _mapper.Map<IEnumerable<CategoryResultDTO>>(categories);
            return categoriesResult;
        }

        public async Task<ProductResultDTO?> GetProductByIdAsync(int id)
        {
            // Create a BaseSpecification to replace ProductWithBrandAndTypeSpecifications
            var spec = new BaseSpecification<Product>(p => p.Id == id);
            spec.AddInclude(p => p.Brand);
            spec.AddInclude(p => p.Category);
            
            var product = await _unitOfWork.GetRepository<Product>().GetAsync(spec);
            return product is null ? throw new ProductNotFoundException(id) : _mapper.Map<ProductResultDTO>(product);
        }
    }
}
