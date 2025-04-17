using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Persistence.Repositories
{
    public class BasketRepository : IbasketRepository
    {
        private readonly StoreContext _context;

        public BasketRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<CustomerBasket?> GetBasketAsync(Guid id)
        {
            Console.WriteLine($"Getting basket with ID: {id}");

            try {
                // Direct database query with explicit loading and eager loading of related entities
                var basket = await _context.CustomerBaskets
                    .AsNoTracking()  // Don't track to avoid caching issues
                    .Include(b => b.BasketItems)
                    .ThenInclude(i => i.Product)  // Explicitly include the ProductInCartItem
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (basket == null)
                {
                    Console.WriteLine($"Basket with ID {id} not found");
                    return null;
                }

                // Ensure BasketItems is not null
                if (basket.BasketItems == null)
                {
                    basket.BasketItems = new List<BasketItem>();
                }

                // Log for debugging
                Console.WriteLine($"Found basket with ID {id}. Item count: {basket.BasketItems.Count()}");

                foreach (var item in basket.BasketItems)
                {
                    Console.WriteLine($"Item ID: {item.Id}, ProductID: {item.Product?.ProductId}, Name: {item.Product?.ProductName}");
                }

                return basket;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            Console.WriteLine($"Updating basket with ID: {basket.Id}, Items: {basket.BasketItems?.Count() ?? 0}");

            try
            {
                // First try to find the basket
                var existingBasket = await _context.CustomerBaskets
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(b => b.Id == basket.Id);

                if (existingBasket == null)
                {
                    // Create new basket if doesn't exist
                    Console.WriteLine($"Basket with ID {basket.Id} not found, creating new one");

                    // Ensure all basket items have valid IDs
                    if (basket.BasketItems != null)
                    {
                        foreach (var item in basket.BasketItems)
                        {
                            if (item.Id == Guid.Empty)
                            {
                                item.Id = Guid.NewGuid();
                            }
                            // Ensure the Product is properly set
                            if (item.Product == null)
                            {
                                Console.WriteLine($"Warning: Item {item.Id} has null Product. Creating empty Product.");
                                item.Product = new ProductInCartItem
                                {
                                    ProductId = 0,
                                    ProductName = "Unknown Product",
                                    PictureUrl = ""
                                };
                            }
                            Console.WriteLine($"Adding item: {item.Id}, Product: {item.Product?.ProductName}, Quantity: {item.Quantity}");
                        }
                    }
                    else
                    {
                        // Initialize empty collection if null
                        basket.BasketItems = new List<BasketItem>();
                    }

                    await _context.CustomerBaskets.AddAsync(basket);
                }
                else
                {
                    Console.WriteLine($"Updating existing basket with ID {basket.Id}");

                    // Update basic properties
                    existingBasket.PaymentIntentId = basket.PaymentIntentId;
                    existingBasket.ClientSecret = basket.ClientSecret;
                    existingBasket.DeliveryMethodId = basket.DeliveryMethodId;
                    existingBasket.ShippingPrice = basket.ShippingPrice;

                    // Remove existing items
                    if (existingBasket.BasketItems != null)
                    {
                        foreach (var existingItem in existingBasket.BasketItems.ToList())
                        {
                            _context.BasketItems.Remove(existingItem);
                        }
                    }
                    else
                    {
                        existingBasket.BasketItems = new List<BasketItem>();
                    }

                    // Save changes after removing basket items to avoid conflicts
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Successfully removed existing items from basket");

                    // Add new items
                    if (basket.BasketItems != null)
                    {
                        foreach (var item in basket.BasketItems)
                        {
                            // Ensure the item has a valid ID
                            if (item.Id == Guid.Empty)
                            {
                                item.Id = Guid.NewGuid();
                            }

                            // Ensure the Product is properly set
                            if (item.Product == null)
                            {
                                Console.WriteLine($"Warning: Item {item.Id} has null Product. Creating empty Product.");
                                item.Product = new ProductInCartItem
                                {
                                    ProductId = 0,
                                    ProductName = "Unknown Product",
                                    PictureUrl = ""
                                };
                            }

                            // Create a new BasketItem for the existing basket
                            var newItem = new BasketItem
                            {
                                Id = item.Id,
                                Product = new ProductInCartItem
                                {
                                    ProductId = item.Product.ProductId,
                                    ProductName = item.Product.ProductName,
                                    PictureUrl = item.Product.PictureUrl
                                },
                                Price = item.Price,
                                Quantity = item.Quantity
                            };

                            // Important: Set the CustomerBasketId to make the relationship explicit
                            _context.Entry(newItem).Property("CustomerBasketId").CurrentValue = existingBasket.Id;
                            
                            // Add the item to the basket and to the context
                            existingBasket.BasketItems.Add(newItem);
                            _context.BasketItems.Add(newItem);
                            
                            Console.WriteLine($"Added item: {newItem.Id}, ProductID: {newItem.Product.ProductId}, Name: {newItem.Product.ProductName}");
                        }
                    }

                    // Use the Entry method to correctly track the entity
                    _context.Entry(existingBasket).State = EntityState.Modified;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved changes to database");

                // Reload the basket from database to ensure we have the latest data
                // and include the Product navigation property
                var updatedBasket = await GetBasketAsync(basket.Id);
                if (updatedBasket != null)
                {
                    Console.WriteLine($"Updated basket has {updatedBasket.BasketItems?.Count() ?? 0} items after reload");
                }
                return updatedBasket;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Try an alternative approach if the first one fails
                if (ex.Message.Contains("expected to affect 1 row(s), but actually affected 0 row(s)") || 
                    ex.Message.Contains("optimistic concurrency"))
                {
                    try
                    {
                        Console.WriteLine("Trying alternative approach to update basket");
                        
                        // Delete the basket if it exists
                        var existingBasket = await _context.CustomerBaskets
                            .Include(b => b.BasketItems)
                            .FirstOrDefaultAsync(b => b.Id == basket.Id);
                            
                        if (existingBasket != null)
                        {
                            _context.CustomerBaskets.Remove(existingBasket);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Removed existing basket with ID {basket.Id}");
                        }
                        
                        // Create a fresh basket
                        var newBasket = new CustomerBasket
                        {
                            Id = basket.Id,
                            PaymentIntentId = basket.PaymentIntentId,
                            ClientSecret = basket.ClientSecret,
                            DeliveryMethodId = basket.DeliveryMethodId,
                            ShippingPrice = basket.ShippingPrice,
                            BasketItems = new List<BasketItem>()
                        };
                        
                        // Add items
                        if (basket.BasketItems != null)
                        {
                            foreach (var item in basket.BasketItems)
                            {
                                // Create new instances of all objects to avoid tracking conflicts
                                var newItem = new BasketItem
                                {
                                    Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
                                    Product = new ProductInCartItem
                                    {
                                        ProductId = item.Product?.ProductId ?? 0,
                                        ProductName = item.Product?.ProductName ?? "Unknown",
                                        PictureUrl = item.Product?.PictureUrl ?? ""
                                    },
                                    Price = item.Price,
                                    Quantity = item.Quantity
                                };
                                
                                newBasket.BasketItems.Add(newItem);
                                Console.WriteLine($"Added new item: {newItem.Id}, ProductID: {newItem.Product.ProductId}, Name: {newItem.Product.ProductName}");
                            }
                        }
                        
                        // Add the new basket to the context
                        await _context.CustomerBaskets.AddAsync(newBasket);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Successfully saved new basket to database");
                        
                        // Reload the basket from database
                        return await GetBasketAsync(basket.Id);
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"Alternative approach failed: {innerEx.Message}");
                        Console.WriteLine($"Stack trace: {innerEx.StackTrace}");
                        throw;
                    }
                }
                
                throw;
            }
        }

        public async Task<bool> DeleteBasketAsync(Guid id)
        {
            Console.WriteLine($"Deleting basket with ID: {id}");

            try
            {
                var basket = await _context.CustomerBaskets
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (basket == null)
                {
                    Console.WriteLine($"Basket with ID {id} not found for deletion");
                    return false;
                }

                _context.CustomerBaskets.Remove(basket);
                var result = await _context.SaveChangesAsync() > 0;

                Console.WriteLine($"Basket deletion result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting basket: {ex.Message}");
                return false;
            }
        }

        public async Task<CustomerBasket?> CreateBasketAsync(CustomerBasket basket)
        {
            Console.WriteLine($"Creating new basket with ID: {basket.Id}");

            try
            {
                // Ensure basket has valid ID
                if (basket.Id == Guid.Empty)
                {
                    basket.Id = Guid.NewGuid();
                }

                // Ensure all basket items have valid IDs
                if (basket.BasketItems != null)
                {
                    foreach (var item in basket.BasketItems)
                    {
                        if (item.Id == Guid.Empty)
                        {
                            item.Id = Guid.NewGuid();
                        }
                    }
                }
                else
                {
                    basket.BasketItems = new List<BasketItem>();
                }

                await _context.CustomerBaskets.AddAsync(basket);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully created basket with ID: {basket.Id}");

                // Reload the basket from database
                return await GetBasketAsync(basket.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<int> GetBasketItemCountAsync(Guid basketId)
        {
            var count = await _context.BasketItems
                .CountAsync(i => EF.Property<Guid>(i, "CustomerBasketId") == basketId);

            Console.WriteLine($"Basket {basketId} has {count} items");
            return count;
        }
    }
}