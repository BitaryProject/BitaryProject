using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.BasketModels;
using System.Text.Json;

namespace Presentation
{
    public class BasketController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public BasketController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasketDTO>> Get(string id)
        {
            Console.WriteLine($"Controller: Getting basket with ID {id}");

            try
            {
                var basket = await _serviceManager.BasketService.GetBasketAsync(id);

                if (basket == null)
                {
                    Console.WriteLine($"Basket not found with ID {id}");
                    return NotFound($"Basket with ID {id} not found");
                }

                Console.WriteLine($"Successfully retrieved basket with {basket.Items?.Count() ?? 0} items");
                return Ok(basket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting basket: {ex.Message}");
                return BadRequest($"Error getting basket: {ex.Message}");
            }
        }

        [HttpPost("{basketId}/items")]
        public async Task<ActionResult<object>> AddItem(string basketId, [FromBody] BasketItemDTO itemDto)
        {
            Console.WriteLine($"Controller: Adding item to basket {basketId}");
            Console.WriteLine($"Item details: {JsonSerializer.Serialize(itemDto)}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest("Invalid basket ID format. Please provide a valid GUID.");
                }

                if (itemDto == null)
                {
                    Console.WriteLine("Item data is null");
                    return BadRequest("No item data provided.");
                }

                // Check for required fields
                if (itemDto.ProductId <= 0)
                {
                    Console.WriteLine("Invalid product ID");
                    return BadRequest("Invalid product ID. ProductId must be greater than 0.");
                }

                if (string.IsNullOrEmpty(itemDto.ProductName))
                {
                    Console.WriteLine("Product name is missing");
                    return BadRequest("Product name is required.");
                }

                if (itemDto.Quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity");
                    return BadRequest("Quantity must be greater than 0.");
                }

                // Try to add/update the item
                var result = await _serviceManager.BasketService.UpdateBasketAsync(basketId, itemDto);

                if (result.Success)
                {
                    // Log success details
                    var itemCount = result.Basket?.Items?.Count() ?? 0;
                    Console.WriteLine($"Successfully added/updated item. Basket now has {itemCount} items.");

                    // Return success result with the updated basket
                    return Ok(new { success = true, basket = result.Basket });
                }
                else
                {
                    Console.WriteLine("Failed to update basket. Check server logs for details.");
                    return BadRequest(new { success = false, message = "Failed to update basket. Check server logs for details." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = $"Error adding item: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            Console.WriteLine($"Controller: Deleting basket with ID {id}");

            try
            {
                var result = await _serviceManager.BasketService.DeleteBasketAsync(id);

                if (result == true)
                {
                    Console.WriteLine($"Successfully deleted basket with ID {id}");
                    return NoContent();
                }
                else
                {
                    Console.WriteLine($"Failed to delete basket with ID {id}");
                    return NotFound($"Basket with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting basket: {ex.Message}");
                return BadRequest($"Error deleting basket: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasketDTO>> CreateBasket()
        {
            Console.WriteLine("Controller: Creating new basket");

            try
            {
                var basketDto = await _serviceManager.BasketService.CreateBasketAsync();
                Console.WriteLine($"Successfully created basket with ID {basketDto.Id}");
                return Ok(basketDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating basket: {ex.Message}");
                return BadRequest($"Error creating basket: {ex.Message}");
            }
        }

        [HttpPut("{basketId}/items/{itemId}")]
        public async Task<ActionResult<CustomerBasketDTO>> UpdateItemQuantity(Guid basketId, Guid itemId, [FromBody] UpdateBasketItemModel model)
        {
            Console.WriteLine($"Controller: Updating quantity for item {itemId} in basket {basketId}");

            try
            {
                if (model == null)
                {
                    Console.WriteLine("No update model provided");
                    return BadRequest("No data provided.");
                }

                var updatedBasket = await _serviceManager.BasketService.UpdateItemQuantityAsync(basketId, itemId, model);

                if (updatedBasket == null)
                {
                    Console.WriteLine("Basket or item not found");
                    return NotFound("Basket or item not found.");
                }

                Console.WriteLine($"Successfully updated item quantity. Basket now has {updatedBasket.Items?.Count() ?? 0} items");
                return Ok(updatedBasket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item quantity: {ex.Message}");
                return BadRequest($"Error updating item quantity: {ex.Message}");
            }
        }

        [HttpDelete("{basketId}/items/{itemId}")]
        public async Task<ActionResult<CustomerBasketDTO>> RemoveItem(Guid basketId, Guid itemId)
        {
            Console.WriteLine($"Controller: Removing item {itemId} from basket {basketId}");

            try
            {
                var updatedBasket = await _serviceManager.BasketService.RemoveItemAsync(basketId, itemId);

                if (updatedBasket == null)
                {
                    Console.WriteLine("Basket or item not found");
                    return NotFound("Basket or item not found.");
                }

                Console.WriteLine($"Successfully removed item. Basket now has {updatedBasket.Items?.Count() ?? 0} items");
                return Ok(updatedBasket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
                return BadRequest($"Error removing item: {ex.Message}");
            }
        }

        [HttpGet("{basketId}/items")]
        public async Task<ActionResult<IEnumerable<BasketItemDTO>>> GetBasketItems(string basketId)
        {
            Console.WriteLine($"Controller: Getting items for basket {basketId}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest("Invalid basket ID format.");
                }

                var basket = await _serviceManager.BasketService.GetBasketAsync(basketId);

                if (basket == null)
                {
                    Console.WriteLine($"Basket not found with ID {basketId}");
                    return NotFound("Basket not found.");
                }

                // Debug information
                var itemCount = basket.Items?.Count() ?? 0;
                Console.WriteLine($"GetBasketItems: Found {itemCount} items in basket {basketId}");

                if (basket.Items != null && basket.Items.Any())
                {
                    foreach (var item in basket.Items)
                    {
                        Console.WriteLine($"Item: {item.Id}, Product: {item.ProductName}, Quantity: {item.Quantity}");
                    }
                }

                return Ok(basket.Items ?? Enumerable.Empty<BasketItemDTO>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving basket items: {ex.Message}");
                return BadRequest($"Error retrieving basket items: {ex.Message}");
            }
        }

        // Debug endpoint to directly query the database
        [HttpGet("debug/{basketId}")]
        public async Task<ActionResult<object>> DebugBasket(string basketId)
        {
            try
            {
                if (!Guid.TryParse(basketId, out var basketGuid))
                {
                    return BadRequest("Invalid basket ID format");
                }

                // Get the raw data from repository
                var result = await _serviceManager.BasketService.GetDebugBasketInfoAsync(basketGuid);
                
                return Ok(new { 
                    basketExists = result.BasketExists,
                    basketId = basketId,
                    itemCount = result.ItemCount,
                    basketItems = result.BasketItems,
                    basketDtoItems = result.BasketDtoItems
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error debugging basket: {ex.Message}");
                return BadRequest($"Error debugging basket: {ex.Message}");
            }
        }

        // Test endpoint for easy debugging
        [HttpGet("test/{basketId}")]
        public async Task<ActionResult<object>> TestAddItem(string basketId)
        {
            try
            {
                Console.WriteLine($"Test endpoint: Adding test item to basket {basketId}");
                
                // Create a test product
                var testItem = new BasketItemDTO
                {
                    Id = Guid.NewGuid(),
                    ProductId = 1,
                    ProductName = "Test Product",
                    PictureUrl = "https://example.com/test.jpg",
                    Price = 10.99m,
                    Quantity = 1
                };
                
                Console.WriteLine($"Test item created: {JsonSerializer.Serialize(testItem)}");
                
                if (!Guid.TryParse(basketId, out var _))
                {
                    // If basketId is invalid, create a new basket
                    Console.WriteLine("Invalid basket ID, creating a new basket");
                    var newBasket = await _serviceManager.BasketService.CreateBasketAsync();
                    basketId = newBasket.Id;
                    Console.WriteLine($"New basket created with ID: {basketId}");
                }
                
                // Add the item to the basket
                var result = await _serviceManager.BasketService.UpdateBasketAsync(basketId, testItem);
                
                if (result.Success)
                {
                    Console.WriteLine("Test successful: Item added to basket");
                    return Ok(new { 
                        success = true, 
                        message = "Test item successfully added to basket",
                        basketId = basketId,
                        basket = result.Basket
                    });
                }
                else
                {
                    Console.WriteLine("Test failed: Could not add item to basket");
                    return BadRequest(new { 
                        success = false, 
                        message = "Test failed: Could not add item to basket", 
                        basketId = basketId 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { 
                    success = false, 
                    message = $"Test error: {ex.Message}" 
                });
            }
        }
    }
}