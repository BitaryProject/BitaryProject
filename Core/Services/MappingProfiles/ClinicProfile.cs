using AutoMapper;
using Domain.Entities.ClinicEntities;
using Shared.ClinicModels;

namespace Services.MappingProfiles
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile()
        {


            /*
            CreateMap<Clinic, ClinicDTO>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => 
                    src.Owner != null ? $"{src.Owner.FirstName} {src.Owner.LastName}" : string.Empty));
                
            CreateMap<ClinicDTO, Clinic>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Doctors, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore());
                
            CreateMap<ClinicRequestDTO, Clinic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ClinicStatus.Pending))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Doctors, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore());
                
            CreateMap<ClinicAddress, ClinicAddressDTO>().ReverseMap();

            */


            CreateMap<Clinic, ClinicDTO>()
               .ForMember(dest => dest.OwnerName, opt => opt.Ignore()); // We'll set this manually

            CreateMap<ClinicDTO, Clinic>()
                .ForMember(dest => dest.Doctors, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore());

            CreateMap<ClinicRequestDTO, Clinic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ClinicStatus.Pending))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.Doctors, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore());

            // New mapping for ClinicUpdateDTO
            CreateMap<ClinicUpdateDTO, Clinic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Doctors, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore());

            CreateMap<ClinicAddress, ClinicAddressDTO>().ReverseMap();
        }
    }
}
