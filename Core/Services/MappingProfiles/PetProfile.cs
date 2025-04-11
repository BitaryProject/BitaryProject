using Domain.Entities.PetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.PetModels;

namespace Services.MappingProfiles
{
        public class PetProfile : Profile
        {
            public PetProfile()
            {
                CreateMap<Pet, PetProfileDTO>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetName))
                    .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                    .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                    .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                    .ReverseMap();
            }
        }
    }
