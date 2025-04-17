using AutoMapper;
using Shared.OrderModels;
using Core.Domain.Entities.SecurityEntities;

namespace Core.Services.MappingProfiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddressDTO, Core.Domain.Entities.SecurityEntities.Address>().ReverseMap();
        }
    }
}
