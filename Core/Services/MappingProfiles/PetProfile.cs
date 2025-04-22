using AutoMapper;
using Domain.Entities.PetEntities;
using Shared.PetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.MappingProfiles
{
    public class PetProfile : Profile
    {
        public PetProfile()
        {
            CreateMap<Pet, PetDTO>();
            CreateMap<CreatePetDTO, Pet>();
            CreateMap<UpdatePetDTO, Pet>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
