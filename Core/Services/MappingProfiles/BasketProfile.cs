
using Domain.Entities.BasketEntities;
using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
           
            CreateMap<CustomerBasket, CustomerBasketDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.BasketItems))
                .ReverseMap();


            CreateMap<BasketItem, BasketItemDTO>()

                .ForMember(dest => dest.Id,
                           opt => opt.MapFrom(src => src.Id))


                .ForMember(dest => dest.ProductId,
                           opt => opt.MapFrom(src => src.Product.ProductId))
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PictureUrl,
                           opt => opt.MapFrom(src => src.Product.PictureUrl))

                .ReverseMap() 
                .ForPath(dest => dest.Product.ProductId,
                         opt => opt.MapFrom(src => src.ProductId))
                .ForPath(dest => dest.Product.ProductName,
                         opt => opt.MapFrom(src => src.ProductName))
                .ForPath(dest => dest.Product.PictureUrl,
                         opt => opt.MapFrom(src => src.PictureUrl));
        }
    }

}
