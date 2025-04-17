using Core.Domain.Contracts;
using Core.Domain.Entities.BasketEntities;
using Domain.Exceptions;
using Shared.BasketModels;
using AutoMapper;
using Core.Services.Abstractions;

namespace Core.Services
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
            if (!Guid.TryParse(id, out var basketGuid))
                throw new BasketNotFoundException(id);

            var basket = await _basketRepository.GetBasketAsync(basketGuid);
            return basket is null
                ? throw new BasketNotFoundException(id)
                : _mapper.Map<CustomerBasketDTO>(basket);
        }


        public async Task<CustomerBasketDTO?> UpdateBasketAsync(string basketId, BasketItemDTO itemDto)
        {
            if (!Guid.TryParse(basketId, out var basketGuid))
                throw new ArgumentException("Invalid basket ID format.");

            var newItem = new BasketItem(
                product: new ProductInCartItem(
                    itemDto.ProductId,
                    itemDto.ProductName,
                    itemDto.PictureUrl
                ),
                quantity: itemDto.Quantity,
                price: itemDto.Price
            );

            var existingBasket = await _basketRepository.GetBasketAsync(basketGuid);

            if (existingBasket == null)
            {
                existingBasket = new CustomerBasket { Id = basketGuid };
                await _basketRepository.CreateBasketAsync(existingBasket); 
            }

            var existingItem = existingBasket.BasketItems
                .FirstOrDefault(i => i.Product.ProductId == newItem.Product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                existingBasket.BasketItems.Add(newItem);
            }

            var updateResult = await _basketRepository.UpdateBasketAsync(existingBasket);

            if (updateResult == null)
                throw new Exception("Failed to update basket.");

            return _mapper.Map<CustomerBasketDTO>(updateResult);
        }

        public async Task<CustomerBasketDTO?> CreateBasketAsync()
        {
            var newBasket = new CustomerBasket
            {
                Id = Guid.NewGuid(),
                BasketItems = new List<BasketItem>()
            };

            var createdBasket = await _basketRepository.CreateBasketAsync(newBasket);

            if (createdBasket == null)
                throw new Exception("Failed to create basket.");

            return _mapper.Map<CustomerBasketDTO>(createdBasket);
        }

        public async Task<CustomerBasketDTO?> UpdateItemQuantityAsync(Guid basketId, Guid itemId, UpdateBasketItemModel model)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            item.Quantity = model.Quantity;

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
            if (updatedBasket == null)
                throw new Exception("Failed to update item quantity in basket.");

            return _mapper.Map<CustomerBasketDTO>(updatedBasket);
        }

        public async Task<CustomerBasketDTO?> RemoveItemAsync(Guid basketId, Guid itemId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            basket.BasketItems.Remove(item);

            var updated = await _basketRepository.UpdateBasketAsync(basket);
            if (updated == null)
                throw new Exception("Failed to remove item from basket.");

            return _mapper.Map<CustomerBasketDTO>(updated);
        }
    }
}
