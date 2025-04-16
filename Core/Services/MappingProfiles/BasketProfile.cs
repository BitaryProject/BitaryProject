using AutoMapper;
using Domain.Entities.BasketEntities;
using Shared.BasketModels;
using System.Collections.Generic;

namespace Services.MappingProfiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            // Map from entity to DTO
            CreateMap<CustomerBasket, CustomerBasketDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => 
                    src.BasketItems != null && src.BasketItems.Any() 
                        ? src.BasketItems 
                        : new List<BasketItem>()))
                .ConstructUsing((src, ctx) => new CustomerBasketDTO 
                {
                    Id = src.Id.ToString(),
                    Items = src.BasketItems != null && src.BasketItems.Any()
                        ? ctx.Mapper.Map<IEnumerable<BasketItemDTO>>(src.BasketItems)
                        : new List<BasketItemDTO>(),
                    PaymentIntentId = src.PaymentIntentId,
                    ClientSecret = src.ClientSecret,
                    DeliveryMethodId = src.DeliveryMethodId,
                    ShippingPrice = src.ShippingPrice
                });

            // Map from BasketItem entity to BasketItemDTO
            CreateMap<BasketItem, BasketItemDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductId : 0))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.PictureUrl : string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            // Map from DTO to entity
            CreateMap<BasketItemDTO, BasketItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
                    src.Id == Guid.Empty ? Guid.NewGuid() : src.Id))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new ProductInCartItem
                {
                    ProductId = src.ProductId,
                    ProductName = src.ProductName,
                    PictureUrl = src.PictureUrl
                }))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            // Map from DTO to entity
            CreateMap<CustomerBasketDTO, CustomerBasket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ParseGuidOrNew(src.Id)))
                .ForMember(dest => dest.BasketItems, opt => opt.MapFrom(src => 
                    src.Items != null && src.Items.Any() 
                        ? src.Items 
                        : new List<BasketItemDTO>()));
        }

        private static Guid ParseGuidOrNew(string id)
        {
            return Guid.TryParse(id, out var guid) ? guid : Guid.NewGuid();
        }
    }
}