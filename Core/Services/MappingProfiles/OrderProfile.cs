using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities.Identity;
using Core.Domain.Entities.OrderEntities;
using Shared.OrderModels;
using AutoMapper;
using OrderAddress = Core.Domain.Entities.OrderEntities.Address;

using UserAddress = Core.Domain.Entities.Identity.Address;
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
                .ForMember(d => d.DeliveryMethod, options => options.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.Total, options => options.MapFrom(s => s.Subtotal + s.DeliveryMethod.Price));

            CreateMap<DeliveryMethod, DeliveryMethodResult>();

            CreateMap<AddressDTO, UserAddress>().ReverseMap();
 
            CreateMap<AddressDTO, OrderAddress>().ReverseMap();
        }
    }
}
