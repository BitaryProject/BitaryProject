using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.BasketModels;
using System.Text.Json;

namespace Services
{
    public class BasketService : IBasketService
    {
        private readonly IbasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IbasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<bool?> DeleteBasketAsync(string id)
        {
            if (!Guid.TryParse(id, out var basketGuid))
                throw new Exception($"Invalid basket Id format: {id}");

            return await _basketRepository.DeleteBasketAsync(basketGuid);
        }

        public async Task<CustomerBasketDTO?> GetBasketAsync(string id)
        {
            Console.WriteLine($"BasketService: Getting basket with ID {id}");

            if (!Guid.TryParse(id, out var basketGuid))
            {
                Console.WriteLine($"Invalid basket ID format: {id}");
                throw new BasketNotFoundException(id);
            }

            var basket = await _basketRepository.GetBasketAsync(basketGuid);

            if (basket == null)
            {
                Console.WriteLine($"Basket not found with ID: {id}");
                throw new BasketNotFoundException(id);
            }

            // Debug logging
            Console.WriteLine($"Found basket entity with ID {id}");
            Console.WriteLine($"Basket items count: {basket.BasketItems?.Count() ?? 0}");

            // Log basket items for debugging
            if (basket.BasketItems != null && basket.BasketItems.Any())
            {
                foreach (var item in basket.BasketItems)
                {
                    Console.WriteLine($"Item: {item.Id}, Product: {item.Product?.ProductName}, Quantity: {item.Quantity}");
                }
            }
            else
            {
                Console.WriteLine("Basket has no items");
            }

            // Map to DTO
            var basketDto = _mapper.Map<CustomerBasketDTO>(basket);

            // Additional logging for the mapped DTO
            Console.WriteLine($"Mapped DTO items count: {basketDto.Items?.Count() ?? 0}");

            // Log mapped DTO items for debugging
            if (basketDto.Items != null && basketDto.Items.Any())
            {
                foreach (var item in basketDto.Items)
                {
                    Console.WriteLine($"DTO Item: {item.Id}, Product: {item.ProductName}, Quantity: {item.Quantity}");
                }
            }
            else
            {
                Console.WriteLine("DTO has no items");
            }

            return basketDto;
        }

        public async Task<(bool Success, CustomerBasketDTO? Basket)> UpdateBasketAsync(string basketId, BasketItemDTO itemDto)
        {
            Console.WriteLine($"BasketService: Adding/updating item in basket {basketId}");
            Console.WriteLine($"Item details: {JsonSerializer.Serialize(itemDto)}");

            if (!Guid.TryParse(basketId, out var basketGuid))
            {
                Console.WriteLine("Invalid basket ID format");
                throw new ArgumentException("Invalid basket ID format.");
            }

            try
            {
                // Validate item data
                if (itemDto == null)
                {
                    Console.WriteLine("Item data is null");
                    return (false, null);
                }

                if (itemDto.ProductId <= 0)
                {
                    Console.WriteLine("Invalid product ID");
                    return (false, null);
                }

                // First get the existing basket
                Console.WriteLine($"Retrieving basket with ID {basketId}");
                var existingBasket = await _basketRepository.GetBasketAsync(basketGuid);

                if (existingBasket == null)
                {
                    // Create new basket if it doesn't exist
                    Console.WriteLine($"Creating new basket with ID {basketId}");
                    existingBasket = new CustomerBasket
                    {
                        Id = basketGuid,
                        BasketItems = new List<BasketItem>()
                    };
                }
                else
                {
                    Console.WriteLine($"Found existing basket with {existingBasket.BasketItems?.Count() ?? 0} items");
                }

                // Ensure basket items collection is initialized
                if (existingBasket.BasketItems == null)
                {
                    Console.WriteLine("Basket items collection was null, initializing it");
                    existingBasket.BasketItems = new List<BasketItem>();
                }

                // Create a new basket item
                var newItem = new BasketItem
                {
                    Id = itemDto.Id == Guid.Empty ? Guid.NewGuid() : itemDto.Id,
                    Product = new ProductInCartItem(
                        itemDto.ProductId,
                        itemDto.ProductName,
                        itemDto.PictureUrl
                    ),
                    Price = itemDto.Price,
                    Quantity = itemDto.Quantity
                };

                Console.WriteLine($"Created new basket item with ID {newItem.Id}, Product ID {newItem.Product.ProductId}, Quantity {newItem.Quantity}");

                // Check if the item already exists in the basket
                var existingItem = existingBasket.BasketItems
                    .FirstOrDefault(i => i.Product.ProductId == itemDto.ProductId);

                if (existingItem != null)
                {
                    Console.WriteLine($"Found existing item with product ID {itemDto.ProductId}, updating quantity from {existingItem.Quantity} to {existingItem.Quantity + itemDto.Quantity}");
                    existingItem.Quantity += itemDto.Quantity;
                    existingItem.Price = itemDto.Price; // Update price in case it changed
                }
                else
                {
                    Console.WriteLine($"Adding new item with product ID {itemDto.ProductId}");
                    existingBasket.BasketItems.Add(newItem);
                }

                // Verify the items are correctly added to the basket
                Console.WriteLine($"Basket now has {existingBasket.BasketItems.Count()} items before repository update");
                foreach (var item in existingBasket.BasketItems)
                {
                    Console.WriteLine($"Basket item before update: ID={item.Id}, ProductID={item.Product?.ProductId}, Name={item.Product?.ProductName}, Quantity={item.Quantity}");
                }

                // Update the basket in the repository
                Console.WriteLine("Updating basket in repository");
                var updatedBasket = await _basketRepository.UpdateBasketAsync(existingBasket);

                if (updatedBasket == null)
                {
                    Console.WriteLine("Failed to update basket in repository");
                    return (false, null);
                }

                // Verify the items after database update
                Console.WriteLine($"Updated basket has {updatedBasket.BasketItems?.Count() ?? 0} items after repository update");
                if (updatedBasket.BasketItems != null && updatedBasket.BasketItems.Any())
                {
                    foreach (var item in updatedBasket.BasketItems)
                    {
                        Console.WriteLine($"Basket item after update: ID={item.Id}, ProductID={item.Product?.ProductId}, Name={item.Product?.ProductName}, Quantity={item.Quantity}");
                    }
                }
                else
                {
                    Console.WriteLine("WARNING: Basket has no items after repository update");
                }

                // Map and return the updated basket
                var basketDto = _mapper.Map<CustomerBasketDTO>(updatedBasket);
                Console.WriteLine($"Successfully updated basket, now contains {basketDto.Items?.Count() ?? 0} items");

                // Check if mapping was successful
                if (basketDto.Items == null || !basketDto.Items.Any())
                {
                    Console.WriteLine("Warning: Mapped basket DTO has no items even though the repository returned items");
                    Console.WriteLine($"Original basket items: {updatedBasket.BasketItems?.Count() ?? 0}, DTO items: {basketDto.Items?.Count() ?? 0}");
                }
                else
                {
                    Console.WriteLine("Successfully mapped basket items to DTO");
                    foreach (var item in basketDto.Items)
                    {
                        Console.WriteLine($"DTO item: ID={item.Id}, ProductID={item.ProductId}, Name={item.ProductName}, Quantity={item.Quantity}");
                    }
                }

                return (true, basketDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return (false, null);
            }
        }

        public async Task<CustomerBasketDTO> CreateBasketAsync()
        {
            Console.WriteLine("Creating new basket");

            var newBasket = new CustomerBasket
            {
                Id = Guid.NewGuid(),
                BasketItems = new List<BasketItem>()
            };

            var createdBasket = await _basketRepository.CreateBasketAsync(newBasket);

            if (createdBasket == null)
            {
                Console.WriteLine("Failed to create basket");
                throw new Exception("Failed to create basket.");
            }

            Console.WriteLine($"Successfully created basket with ID {createdBasket.Id}");
            return _mapper.Map<CustomerBasketDTO>(createdBasket);
        }

        public async Task<CustomerBasketDTO?> UpdateItemQuantityAsync(Guid basketId, Guid itemId, UpdateBasketItemModel model)
        {
            Console.WriteLine($"Updating quantity for item {itemId} in basket {basketId}");

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null)
            {
                Console.WriteLine($"Basket not found with ID {basketId}");
                return null;
            }

            if (basket.BasketItems == null || !basket.BasketItems.Any())
            {
                Console.WriteLine($"No items found in basket {basketId}");
                return _mapper.Map<CustomerBasketDTO>(basket);
            }

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                Console.WriteLine($"Item not found with ID {itemId} in basket {basketId}");
                return null;
            }

            Console.WriteLine($"Updating item quantity from {item.Quantity} to {model.Quantity}");
            item.Quantity = model.Quantity;

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket == null)
            {
                Console.WriteLine("Failed to update item quantity");
                throw new Exception("Failed to update item quantity in basket.");
            }

            Console.WriteLine("Successfully updated item quantity");
            return _mapper.Map<CustomerBasketDTO>(updatedBasket);
        }

        public async Task<CustomerBasketDTO?> RemoveItemAsync(Guid basketId, Guid itemId)
        {
            Console.WriteLine($"Removing item {itemId} from basket {basketId}");

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null)
            {
                Console.WriteLine($"Basket not found with ID {basketId}");
                return null;
            }

            if (basket.BasketItems == null || !basket.BasketItems.Any())
            {
                Console.WriteLine($"No items found in basket {basketId}");
                return _mapper.Map<CustomerBasketDTO>(basket);
            }

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                Console.WriteLine($"Item not found with ID {itemId} in basket {basketId}");
                return null;
            }

            Console.WriteLine($"Removing item {itemId} from basket");
            basket.BasketItems.Remove(item);

            var updated = await _basketRepository.UpdateBasketAsync(basket);

            if (updated == null)
            {
                Console.WriteLine("Failed to remove item from basket");
                throw new Exception("Failed to remove item from basket.");
            }

            Console.WriteLine("Successfully removed item from basket");
            return _mapper.Map<CustomerBasketDTO>(updated);
        }

        public async Task<int> GetBasketItemCountAsync(Guid basketId)
        {
            return await _basketRepository.GetBasketItemCountAsync(basketId);
        }

        public async Task<(bool BasketExists, int ItemCount, object BasketItems, object BasketDtoItems)> GetDebugBasketInfoAsync(Guid basketId)
        {
            Console.WriteLine($"[DEBUG] Getting debug info for basket {basketId}");
            
            try
            {
                // Get the basket directly from the repository
                var basket = await _basketRepository.GetBasketAsync(basketId);
                
                if (basket == null)
                {
                    Console.WriteLine($"[DEBUG] Basket {basketId} not found");
                    return (false, 0, null, null);
                }
                
                // Log the raw basket data
                Console.WriteLine($"[DEBUG] Found basket with ID {basketId}");
                Console.WriteLine($"[DEBUG] Raw basket items count: {basket.BasketItems?.Count() ?? 0}");
                
                // Get the item details - convert to object list to avoid type mismatch
                object basketItemsData = basket.BasketItems?
                    .Select(i => new { 
                        Id = i.Id, 
                        ProductId = i.Product?.ProductId,
                        ProductName = i.Product?.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price 
                    })
                    .ToList() as object ?? new List<object>();
                
                // Map to DTO to check the mapping
                var basketDto = _mapper.Map<CustomerBasketDTO>(basket);
                
                // Convert to object list to avoid type mismatch
                object basketDtoItems = basketDto.Items?
                    .Select(i => new {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    })
                    .ToList() as object ?? new List<object>();
                
                Console.WriteLine($"[DEBUG] DTO items count: {basketDto.Items?.Count() ?? 0}");
                
                return (
                    true, 
                    basket.BasketItems?.Count() ?? 0, 
                    basketItemsData, 
                    basketDtoItems
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Error debugging basket: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}