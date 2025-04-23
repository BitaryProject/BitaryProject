using AutoMapper;
using Domain.Entities.DoctorEntites;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class DoctorScheduleProfile : Profile
    {
        public DoctorScheduleProfile()
        {
            CreateMap<DoctorSchedule, DoctorScheduleDTO>()
                .ForMember(dest => dest.StartTimeString, opt => opt.MapFrom(src => $"{src.StartTime.Hours:D2}:{src.StartTime.Minutes:D2}"))
                .ForMember(dest => dest.EndTimeString, opt => opt.MapFrom(src => $"{src.EndTime.Hours:D2}:{src.EndTime.Minutes:D2}"))
                .ForMember(dest => dest.DoctorName, opt => opt.Ignore());
                
            CreateMap<DoctorScheduleDTO, DoctorSchedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
        }
    }
}
