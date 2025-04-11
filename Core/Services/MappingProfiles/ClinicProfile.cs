using Domain.Entities.ClinicEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.ClinicModels;
using Shared.OrderModels;
namespace Services.MappingProfiles
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile()
        {
            CreateMap<Clinic, ClinicDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.ClinicName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ExaminationFee, opt => opt.MapFrom(src => src.ExaminationFee))
                .ReverseMap();

        }
    }
}
