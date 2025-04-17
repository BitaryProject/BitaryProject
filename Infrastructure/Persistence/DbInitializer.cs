global using Core.Domain.Contracts;
global using Core.Domain.Entities.ProductEntities;
global using Microsoft.EntityFrameworkCore;
global using Infrastructure.Persistence.Data;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities.OrderEntities;
using Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Core.Domain.Entities.SecurityEntities;
using Microsoft.Data.SqlClient;


namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly StoreContext _storeContext;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        //private readonly NewModuleContext _newModuleContext;




        public DbInitializer(StoreContext storeContext,
            RoleManager<IdentityRole> roleManager,
            //NewModuleContext newModuleContext,
            UserManager<User> userManager)   
        {
            _storeContext = storeContext;
            _roleManager = roleManager;
            _userManager = userManager;
            //_newModuleContext = newModuleContext;
        }

        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Attempting to connect to database...");
                bool hasConnection = false;
                
                try {
                    // Test the connection before attempting any operations
                    hasConnection = await _storeContext.Database.CanConnectAsync();
                    Console.WriteLine($"Database connection test: {(hasConnection ? "Successful" : "Failed")}");
                    
                    if (!hasConnection) {
                        Console.WriteLine("Cannot connect to database. Check connection string and network.");
                        return;
                    }
                }
                catch (Exception connEx) {
                    Console.WriteLine($"Connection test error: {connEx.Message}");
                    Console.WriteLine($"Connection error type: {connEx.GetType().Name}");
                    if (connEx.InnerException != null) {
                        Console.WriteLine($"Inner exception: {connEx.InnerException.Message}");
                    }
                    return;
                }
                
                if (_storeContext.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    await _storeContext.Database.MigrateAsync();
                    Console.WriteLine("Migrations completed successfully");
                }

                // Check if data already exists before trying to seed
                if (!_storeContext.ProductCategories.Any())
                {
                    try
                    {
                        Console.WriteLine("Seeding product categories...");
                        // Use a more robust path resolution for production environments
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string seedingPath = Path.Combine(basePath, "Persistence", "Data", "Seeding");
                        
                        // Fall back to relative path if the direct path doesn't exist
                        if (!Directory.Exists(seedingPath))
                        {
                            seedingPath = @"..\Infrastructure\Persistence\Data\Seeding";
                            if (!Directory.Exists(seedingPath))
                            {
                                Console.WriteLine($"Seeding directory not found at: {seedingPath}");
                                // Try to find the seeding directory
                                seedingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Seeding");
                                if (!Directory.Exists(seedingPath))
                                {
                                    seedingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Data", "Seeding");
                                }
                                Console.WriteLine($"Trying alternative path: {seedingPath}");
                            }
                        }
                        
                        string categoryPath = Path.Combine(seedingPath, "category.json");
                        Console.WriteLine($"Looking for category.json at: {categoryPath}");
                        
                        if (File.Exists(categoryPath))
                        {
                            var typesData = await File.ReadAllTextAsync(categoryPath);
                            var types = JsonSerializer.Deserialize<List<ProductCategory>>(typesData);

                            if (types is not null && types.Any())
                            {
                                await _storeContext.ProductCategories.AddRangeAsync(types);
                                await _storeContext.SaveChangesAsync();
                                Console.WriteLine($"Added {types.Count} product categories");
                            }
                        }
                        else
                        {
                            Console.WriteLine("category.json file not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue - seeding is not critical to app operation
                        Console.WriteLine($"Error seeding product categories: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
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

                if (!_storeContext.DeliveryMethods.Any())
                {
                    var data = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\delivery.json");

                    var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(data);

                    if (methods is not null && methods.Any())
                    {
                        await _storeContext.DeliveryMethods.AddRangeAsync(methods);
                        await _storeContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                Console.WriteLine($"Error type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().Name}");
                }
                // Re-throw only critical exceptions that should stop the application
                if (ex is DbUpdateException || ex is SqlException)
                    throw;
            }
        }

        public async Task InitializeIdentityAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                
                // Healthcare-related roles
                await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                await _roleManager.CreateAsync(new IdentityRole("PetOwner"));
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
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
                    Email = "abdelrhmanali2119@gmail.com",
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


