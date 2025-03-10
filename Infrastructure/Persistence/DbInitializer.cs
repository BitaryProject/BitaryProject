global using Domain.Contracts;
global using Domain.Entities.ProductEntities;
global using Microsoft.EntityFrameworkCore;
global using Persistence.Data;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly StoreContext _storeContext;
        public DbInitializer(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_storeContext.Database.GetPendingMigrations().Any())
                    await _storeContext.Database.MigrateAsync();


                if (!_storeContext.ProductCategories.Any())

                {
                    var typesData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\category.json");

                    var types = JsonSerializer.Deserialize<List<ProductCategory>>(typesData);

                    if (types is not null && types.Any())
                    {
                        await _storeContext.ProductCategories.AddRangeAsync(types);

                        await _storeContext.SaveChangesAsync();
                    }
                }


                if (!_storeContext.ProductBrands.Any())
                {
                    var brandsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\brands.json");

                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    if (brands is not null && brands.Any())
                    {
                        await _storeContext.ProductBrands.AddRangeAsync(brands);
                        await _storeContext.SaveChangesAsync();
                    }



                }


                if (!_storeContext.Products.Any())
                {
                    var productsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\products.json");

                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    if (products is not null && products.Any())
                    {
                        await _storeContext.Products.AddRangeAsync(products);
                        await _storeContext.SaveChangesAsync();
                    }
                }


            }


            catch (Exception)
            {
                throw;
            }
        }
    }
}
//..\Infrastructure\Persistence\Data\Seeding\products.json

