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
        public async Task<ActionResult<object>> AddItem(string basketId, [FromBody] AddItemRequestDTO request)
        {
            Console.WriteLine($"Controller: Adding item to basket {basketId}");
            Console.WriteLine($"Item details: {JsonSerializer.Serialize(request)}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest("Invalid basket ID format. Please provide a valid GUID.");
                }

                if (request == null)
                {
                    Console.WriteLine("Item data is null");
                    return BadRequest("No item data provided.");
                }

                // Check for required fields
                if (request.ProductId <= 0)
                {
                    Console.WriteLine("Invalid product ID");
                    return BadRequest("Invalid product ID. ProductId must be greater than 0.");
                }

                if (request.Quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity");
                    return BadRequest("Quantity must be greater than 0.");
                }

                // Try to add/update the item
                var success = await _serviceManager.BasketService.AddItemToBasketAsync(basketId, request.ProductId, request.Quantity);

                if (success)
                {
                    Console.WriteLine("Successfully added/updated item");
                    return Ok(new { status = "success" });
                }
                else
                {
                    Console.WriteLine("Failed to update basket. Check server logs for details.");
                    return BadRequest(new { status = "failed", message = "Failed to update basket. Check server logs for details." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { status = "failed", message = $"Error adding item: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            Console.WriteLine($"Controller: Deleting basket with ID {id}");

            try
            {
                var success = await _serviceManager.BasketService.DeleteBasketAsync(id);

                if (success)
                {
                    Console.WriteLine($"Successfully deleted basket with ID {id}");
                    return Ok(new { status = "success" });
                }
                else
                {
                    Console.WriteLine($"Failed to delete basket with ID {id}");
                    return NotFound(new { status = "failed", message = $"Basket with ID {id} not found" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting basket: {ex.Message}");
                return BadRequest(new { status = "failed", message = $"Error deleting basket: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateBasket()
        {
            Console.WriteLine("Controller: Creating new basket");

            try
            {
                var result = await _serviceManager.BasketService.CreateBasketAsync();
                
                if (result.Success)
                {
                    Console.WriteLine($"Successfully created basket with ID {result.BasketId}");
                    return Ok(new { status = "success", basketId = result.BasketId });
                }
                else
                {
                    Console.WriteLine("Failed to create basket");
                    return BadRequest(new { status = "failed", message = "Failed to create basket" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating basket: {ex.Message}");
                return BadRequest(new { status = "failed", message = $"Error creating basket: {ex.Message}" });
            }
        }

        [HttpPut("{basketId}/items/{productId}")]
        public async Task<ActionResult<object>> UpdateItemQuantity(string basketId, int productId, [FromBody] UpdateBasketItemModel model)
        {
            Console.WriteLine($"Controller: Updating quantity for product {productId} in basket {basketId}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest(new { status = "failed", message = "Invalid basket ID format. Please provide a valid GUID." });
                }

                if (model == null)
                {
                    Console.WriteLine("No update model provided");
                    return BadRequest(new { status = "failed", message = "No data provided." });
                }

                var success = await _serviceManager.BasketService.UpdateItemQuantityAsync(basketId, productId, model.Quantity);

                if (success)
                {
                    Console.WriteLine("Successfully updated item quantity");
                    return Ok(new { status = "success" });
                }
                else
                {
                    Console.WriteLine("Basket or item not found");
                    return NotFound(new { status = "failed", message = "Basket or item not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item quantity: {ex.Message}");
                return BadRequest(new { status = "failed", message = $"Error updating item quantity: {ex.Message}" });
            }
        }

        [HttpDelete("{basketId}/items/{productId}")]
        public async Task<ActionResult<object>> RemoveItem(string basketId, int productId)
        {
            Console.WriteLine($"Controller: Removing product {productId} from basket {basketId}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest(new { status = "failed", message = "Invalid basket ID format. Please provide a valid GUID." });
                }

                var success = await _serviceManager.BasketService.RemoveItemAsync(basketId, productId);

                if (success)
                {
                    Console.WriteLine("Successfully removed item");
                    return Ok(new { status = "success" });
                }
                else
                {
                    Console.WriteLine("Basket or item not found");
                    return NotFound(new { status = "failed", message = "Basket or item not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}`");
                return BadRequest(new { status = "failed", message = $"Error removing item: {ex.Message}" });
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

        [HttpPut("{basketId}/delivery-method")]
        public async Task<ActionResult<object>> UpdateDeliveryMethod(string basketId, [FromBody] UpdateDeliveryMethodRequest request)
        {
            Console.WriteLine($"Controller: Updating delivery method for basket {basketId}");

            try
            {
                if (!Guid.TryParse(basketId, out var _))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return BadRequest(new { status = "failed", message = "Invalid basket ID format. Please provide a valid GUID." });
                }

                if (request == null || request.DeliveryMethodId <= 0)
                {
                    Console.WriteLine("Invalid delivery method ID");
                    return BadRequest(new { status = "failed", message = "Invalid or missing delivery method ID." });
                }

                var success = await _serviceManager.BasketService.UpdateDeliveryMethodAsync(basketId, request.DeliveryMethodId);

                if (success)
                {
                    Console.WriteLine("Successfully updated delivery method");
                    return Ok(new { status = "success" });
                }
                else
                {
                    Console.WriteLine("Failed to update delivery method");
                    return BadRequest(new { status = "failed", message = "Failed to update delivery method. Check server logs for details." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating delivery method: {ex.Message}");
                return BadRequest(new { status = "failed", message = $"Error updating delivery method: {ex.Message}" });
            }
        }
    }
}