using Domain.Entities.BasketEntities;
using Domain.Exceptions;
using Shared.BasketModels;

namespace Services
{
    public class BasketService : IBasketService
    {
        private readonly IbasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketService(IbasketRepository basketRepository, IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }

        public async Task<bool?> DeleteBasketAsync(string id)
        {
            if (!Guid.TryParse(id, out var basketGuid))
                throw new Exception($"Invalid basket Id format: {id}");

            return await basketRepository.DeleteBasketAsync(basketGuid);
        }

        public async Task<CustomerBasketDTO?> GetBasketAsync(string id)
        {
            if (!Guid.TryParse(id, out var basketGuid))
                throw new BasketNotFoundException(id);

            var basket = await basketRepository.GetBasketAsync(basketGuid);
            return basket is null
                ? throw new BasketNotFoundException(id)
                : mapper.Map<CustomerBasketDTO>(basket);
        }

       
        public async Task<CustomerBasketDTO?> UpdateBasketAsync(string basketId, BasketItemDTO itemDto)
        {
            if (!Guid.TryParse(basketId, out var basketGuid))
                throw new Exception($"Invalid basket Id format: {basketId}");

            var newItem = mapper.Map<BasketItem>(itemDto);

            var existingBasket = await basketRepository.GetBasketAsync(basketGuid);
            if (existingBasket == null)
            {
                existingBasket = new CustomerBasket
                {
                    Id = basketGuid,
                    BasketItems = new List<BasketItem>()
                };
            }

            var existingItem = existingBasket.BasketItems
                .FirstOrDefault(i => i.Product.ProductId == newItem.Product.ProductId);

            if (existingItem == null)
            {
                existingBasket.BasketItems.Add(newItem);
            }
            else
            {
                existingItem.Quantity += newItem.Quantity;
                existingItem.Price = newItem.Price;
                existingItem.Product.PictureUrl = newItem.Product.PictureUrl;
                existingItem.Product.ProductName = newItem.Product.ProductName;
            }

            var updatedBasket = await basketRepository.UpdateBasketAsync(existingBasket);
            if (updatedBasket is null)
                throw new Exception("Can't update basket now!");

            return mapper.Map<CustomerBasketDTO>(updatedBasket);
        }






        public async Task<CustomerBasketDTO> CreateBasketAsync()
        {
            
            var newBasket = new CustomerBasket
            {
                Id = Guid.NewGuid(),
                BasketItems = new List<BasketItem>()
            };

           
            var createdBasket = await basketRepository.UpdateBasketAsync(newBasket);
            if (createdBasket is null)
                throw new Exception("Failed to create basket.");


            return mapper.Map<CustomerBasketDTO>(createdBasket);
        }



        public async Task<CustomerBasketDTO?> UpdateItemQuantityAsync(Guid basketId, Guid itemId, UpdateBasketItemModel model)
        {
         
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            item.Quantity = model.Quantity;

            var updatedBasket = await basketRepository.UpdateBasketAsync(basket);
            if (updatedBasket == null)
                throw new Exception("Failed to update item quantity in basket.");

            return mapper.Map<CustomerBasketDTO>(updatedBasket);
        }


        public async Task<CustomerBasketDTO?> RemoveItemAsync(Guid basketId, Guid itemId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            basket.BasketItems.Remove(item);

            var updated = await basketRepository.UpdateBasketAsync(basket);
            if (updated == null)
                throw new Exception("Failed to remove item from basket.");

            return mapper.Map<CustomerBasketDTO>(updated);
        }


    }
}
