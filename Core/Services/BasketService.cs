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
        private readonly IUnitOFWork _unitOfWork;

        public BasketService(IbasketRepository basketRepository, IMapper mapper, IUnitOFWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteBasketAsync(string id)
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

        public async Task<bool> UpdateBasketAsync(string basketId, BasketItemDTO itemDto)
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
                    return false;
                }

                if (itemDto.ProductId <= 0)
                {
                    Console.WriteLine("Invalid product ID");
                    return false;
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
                    return false;
                }

                Console.WriteLine($"Successfully updated basket with {updatedBasket.BasketItems?.Count() ?? 0} items");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<(bool Success, string BasketId)> CreateBasketAsync()
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
                return (false, string.Empty);
            }

            var basketId = createdBasket.Id.ToString();
            Console.WriteLine($"Successfully created basket with ID {basketId}");
            return (true, basketId);
        }

        public async Task<bool> UpdateItemQuantityAsync(string basketId, int productId, int quantity)
        {
            Console.WriteLine($"Updating quantity for product {productId} in basket {basketId}");

            if (!Guid.TryParse(basketId, out var basketGuid))
            {
                Console.WriteLine("Invalid basket ID format");
                return false;
            }

            var basket = await _basketRepository.GetBasketAsync(basketGuid);

            if (basket == null)
            {
                Console.WriteLine($"Basket not found with ID {basketId}");
                return false;
            }

            if (basket.BasketItems == null || !basket.BasketItems.Any())
            {
                Console.WriteLine($"No items found in basket {basketId}");
                return false;
            }

            var item = basket.BasketItems.FirstOrDefault(i => i.Product.ProductId == productId);

            if (item == null)
            {
                Console.WriteLine($"Item with product ID {productId} not found in basket {basketId}");
                return false;
            }

            Console.WriteLine($"Updating item quantity from {item.Quantity} to {quantity}");
            item.Quantity = quantity;

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket == null)
            {
                Console.WriteLine("Failed to update item quantity");
                return false;
            }

            Console.WriteLine("Successfully updated item quantity");
            return true;
        }

        public async Task<bool> RemoveItemAsync(string basketId, int productId)
        {
            Console.WriteLine($"Removing product {productId} from basket {basketId}");

            if (!Guid.TryParse(basketId, out var basketGuid))
            {
                Console.WriteLine("Invalid basket ID format");
                return false;
            }

            var basket = await _basketRepository.GetBasketAsync(basketGuid);

            if (basket == null)
            {
                Console.WriteLine($"Basket not found with ID {basketId}");
                return false;
            }

            if (basket.BasketItems == null || !basket.BasketItems.Any())
            {
                Console.WriteLine($"No items found in basket {basketId}");
                return false;
            }

            var item = basket.BasketItems.FirstOrDefault(i => i.Product.ProductId == productId);

            if (item == null)
            {
                Console.WriteLine($"Item with product ID {productId} not found in basket {basketId}");
                return false;
            }

            Console.WriteLine($"Removing item with product ID {productId} from basket");
            basket.BasketItems.Remove(item);

            var updated = await _basketRepository.UpdateBasketAsync(basket);

            if (updated == null)
            {
                Console.WriteLine("Failed to remove item from basket");
                return false;
            }

            Console.WriteLine("Successfully removed item from basket");
            return true;
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

        public async Task<bool> AddItemToBasketAsync(string basketId, int productId, int quantity)
        {
            Console.WriteLine($"BasketService: Adding item with productId {productId} to basket {basketId}");
            
            if (!Guid.TryParse(basketId, out var basketGuid))
            {
                Console.WriteLine("Invalid basket ID format");
                throw new ArgumentException("Invalid basket ID format.");
            }

            try
            {
                // Validate input data
                if (productId <= 0)
                {
                    Console.WriteLine("Invalid product ID");
                    return false;
                }

                if (quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity");
                    return false;
                }

                // Fetch the product details from the product repository
                var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(productId);
                
                if (product == null)
                {
                    Console.WriteLine($"Product with ID {productId} not found");
                    return false;
                }
                
                Console.WriteLine($"Found product: {product.Name}, Price: {product.Price}");

                // Create a BasketItemDTO with the product details
                var basketItemDto = new BasketItemDTO
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl,
                    Price = product.Price,
                    Quantity = quantity,
                    Description = product.Description
                };

                // Use the existing method to update the basket
                return await UpdateBasketAsync(basketId, basketItemDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item to basket: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId)
        {
            Console.WriteLine($"BasketService: Updating delivery method for basket {basketId} to {deliveryMethodId}");

            try
            {
                if (!Guid.TryParse(basketId, out var basketGuid))
                {
                    Console.WriteLine("Invalid basket ID format");
                    return false;
                }

                // Check if the delivery method exists
                var deliveryMethod = await _unitOfWork.GetRepository<Domain.Entities.OrderEntities.DeliveryMethod, int>()
                    .GetAsync(deliveryMethodId);

                if (deliveryMethod == null)
                {
                    Console.WriteLine($"Delivery method with ID {deliveryMethodId} not found");
                    return false;
                }

                // Get the existing basket
                var basket = await _basketRepository.GetBasketAsync(basketGuid);

                if (basket == null)
                {
                    Console.WriteLine($"Basket with ID {basketId} not found");
                    return false;
                }

                // Update the delivery method
                basket.DeliveryMethodId = deliveryMethodId;

                // Save the changes
                var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

                return updatedBasket != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating delivery method: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}