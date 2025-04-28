using AutoMapper;
using Domain.Entities.ProductEntities;
using Shared.WishListModels;
using System;

namespace Services.MappingProfiles
{
    public class WishListProfile : Profile
    {
        public WishListProfile()
        {
            // Map WishList to WishListDTO
            CreateMap<WishList, WishListDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.WishListItems));
            
            // Map WishListItem to WishListItemDTO
            CreateMap<WishListItem, WishListItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.Product.ProductBrand != null ? src.Product.ProductBrand.Name : null))
                .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Product.ProductCategory != null ? src.Product.ProductCategory.Name : null));
        }
    }
} 