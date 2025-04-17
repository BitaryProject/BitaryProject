using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;

namespace Services.MappingProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetProfile.Name))
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FirstName + " " + src.Doctor.User.LastName));

            CreateMap<AppointmentCreateDTO, Appointment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Scheduled))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration ?? TimeSpan.FromHours(1)));

            CreateMap<AppointmentUpdateDTO, Appointment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.Status) ? AppointmentStatus.Scheduled : Enum.Parse<AppointmentStatus>(src.Status)));
        }
    }
}
