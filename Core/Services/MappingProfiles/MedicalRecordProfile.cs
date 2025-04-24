using AutoMapper;
using Domain.Entities.MedicalRecordEntites;
using Shared.MedicalRecordModels;

namespace Services.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            // Entity to DTO mapping
            CreateMap<MedicalRecord, MedicalRecordDTO>()
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.PetName : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

            // DTO to Entity mapping
            CreateMap<MedicalRecordDTO, MedicalRecord>()
                .ForMember(dest => dest.Pet, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Appointment, opt => opt.Ignore());
            // CreateDTO to Entity mapping
            CreateMap<MedicalRecordCreateDTO, MedicalRecord>()
                .ForMember(dest => dest.Pet, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Appointment, opt => opt.Ignore())
                .ForMember(dest => dest.PetId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
