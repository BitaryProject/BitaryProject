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
using Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.SecurityEntities;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly StoreContext _storeContext;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;



        public DbInitializer(StoreContext storeContext,
            RoleManager<IdentityRole> roleManager, 
            UserManager<User> userManager)   
        {
            _storeContext = storeContext;
            _roleManager = roleManager;
            _userManager = userManager;
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

        public async Task InitializeIdentityAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!_userManager.Users.Any())
            {
                var superAdminUser = new User
                {
                    DisplayName = "Abdelrhman Kamal ElDin",
                    Email = "abdelrhmankamal3@gmail.com",
                    UserName = "AbdelrhmanK12",
                    PhoneNumber = "01129038026"
                }; 
                var adminUser = new User
                {
                    DisplayName = "Abdelrhman Ali",
                    Email = "abdelrhmanali22@gmail.com",
                    UserName = "AbdelrhmanAli22",
                    PhoneNumber = "01142029061"
                };

                await _userManager.CreateAsync(superAdminUser, "Abdo@777");
                await _userManager.CreateAsync(adminUser, "Abdo@888");


                await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

        }
    }
}
//..\Infrastructure\Persistence\Data\Seeding\products.json

