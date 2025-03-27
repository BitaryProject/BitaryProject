using AutoMapper;
using Shared.OrderModels;
using Domain.Entities.SecurityEntities;

namespace Core.Services.MappingProfiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddressDTO, Domain.Entities.SecurityEntities.Address>().ReverseMap();
        }
    }
}
