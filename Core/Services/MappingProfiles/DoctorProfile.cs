//using Domain.Entities.DoctorEntites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Shared.DoctorModels;

//namespace Services.MappingProfiles
//{
//    public class DoctorProfile : Profile
//    {
//        public DoctorProfile()
//        {
//            CreateMap<Doctor, DoctorDTO>()
//                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
//                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
//                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
//                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
//                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
//                .ForMember(dest => dest.ClinicId, opt => opt.MapFrom(src => src.ClinicId))
//                .ReverseMap();
//        }
//    }
//}
