using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Identity;
using Domain.Entities.OrderEntities;
using Shared.OrderModels;
using OrderAddress = Domain.Entities.OrderEntities.Address;

using UserAddress = Domain.Entities.Identity.Address;
namespace Services.MappingProfiles
{
    class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderAddress, AddressDTO>();

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(d => d.ProductId, options => options.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.PictureUrl, options => options.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.ProductName, options => options.MapFrom(s => s.Product.ProductName));

            CreateMap<Order, OrderResult>()
                .ForMember(d => d.PaymentStatus, options => options.MapFrom(s => s.PaymentStatus.ToString()))
                .ForMember(d => d.DeliveryMethod, options => options.MapFrom(s => s.DeliveryMethod != null ? s.DeliveryMethod.ShortName : "None"))
                .ForMember(d => d.Total, options => options.MapFrom(s => s.Subtotal + (s.DeliveryMethod != null ? s.DeliveryMethod.Price : 0)));

            CreateMap<DeliveryMethod, DeliveryMethodResult>();

            CreateMap<AddressDTO, UserAddress>().ReverseMap();
 
            CreateMap<AddressDTO, OrderAddress>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
                
            CreateMap<OrderAddress, AddressDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
        }
    }
}
