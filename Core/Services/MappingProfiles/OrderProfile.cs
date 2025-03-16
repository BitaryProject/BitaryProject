using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;
using Shared.OrderModels;

namespace Services.MappingProfiles
{
    class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>()

                .ForMember(d => d.ProductId,
                options => options.MapFrom(s => s.Product.ProductId))

                .ForMember(d => d.PictureUrl,
                options => options.MapFrom(s => s.Product.PictureUrl))

                .ForMember(d => d.ProductName,
                options => options.MapFrom(s => s.Product.ProductName));



             CreateMap<Order, OrderResult>()


                .ForMember(order => order.PaymentStatus,
                options => options.MapFrom(s => s.ToString()))

                .ForMember(d => d.DeliveryMethod,
                options => options.MapFrom(s => s.DeliveryMethod.ShortName))

                .ForMember(t => t.Total,
                options => options.MapFrom(s => s.Subtotal + s.DeliveryMethod.Price));


            CreateMap<DeliveryMethod, DeliveryMethodResult>();
        }
    }
}
