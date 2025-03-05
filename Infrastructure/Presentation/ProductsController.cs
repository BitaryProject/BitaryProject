using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IServiceManager ServiceManager) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProductResultDTO>>> GetAllProducts([FromQuery]ProductSpecificationsParameters parameters)
        {
            var products = await ServiceManager.ProductService.GetAllProductsAsync(parameters);

            return Ok(products);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandResultDTO>>> GetAllBrands()
        {
            var brands = await ServiceManager.ProductService.GetAllBrandsAsync();

            return Ok(brands);
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryResultDTO>>> GetAllTypes()
        {
            var categories = await ServiceManager.ProductService.GetAllCategoriesAsync();

            return Ok(categories);
        }


        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]

        [HttpGet("Product{id}")]
        public async Task<ActionResult<ProductResultDTO>> GetProductById(int id)
        {
            var product = await ServiceManager.ProductService.GetProductByIdAsync(id);

            return Ok(product);
        }

    }
}
