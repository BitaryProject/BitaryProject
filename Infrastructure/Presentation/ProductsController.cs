using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ErrorModels;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Presentation
{
    [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]

    public class ProductsController : ApiController
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IServiceManager serviceManager, ILogger<ProductsController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProductResultDTO>>> GetAllProducts([FromQuery]ProductSpecificationsParameters parameters)
        {
            try
            {
                _logger.LogInformation("Getting all products with parameters: Brand={BrandId}, Category={CategoryId}, Sort={Sort}, Page={Page}, PageSize={PageSize}", 
                    parameters.BrandId, parameters.CategoryId, parameters.Sort, parameters.PageIndex, parameters.PageSize);
                
                var products = await _serviceManager.ProductService.GetAllProductsAsync(parameters);
                
                _logger.LogInformation("Retrieved {Count} products out of {TotalCount} total", 
                    products.TotalCount, products.TotalCount.ToString());
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                }
                
                // Return a more detailed error in development, but use a generic message in production
                var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = isDevelopment 
                        ? $"Error retrieving products: {ex.Message}" 
                        : "An error occurred while retrieving products. Please try again later."
                });
            }
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandResultDTO>>> GetAllBrands()
        {
            try
            {
                var brands = await _serviceManager.ProductService.GetAllBrandsAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving brands");
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving brands"
                });
            }
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryResultDTO>>> GetAllTypes()
        {
            try
            {
                var categories = await _serviceManager.ProductService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving categories"
                });
            }
        }



        [HttpGet("Product{id}")]
        public async Task<ActionResult<ProductResultDTO>> GetProductById(int id)
        {
            try
            {
                var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = $"An error occurred while retrieving product with ID {id}"
                });
            }
        }

    }
}
