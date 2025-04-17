using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;

namespace Core.Services.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            // Medical Record mappings
            CreateMap<MedicalRecord, MedicalRecordDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetProfile != null ? src.PetProfile.Name : null))
                .ReverseMap();

            CreateMap<MedicalRecordCreateDTO, MedicalRecord>();
            CreateMap<MedicalRecordUpdateDTO, MedicalRecord>();

            // Medical Note mappings
            CreateMap<MedicalNote, MedicalNoteDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                .ReverseMap();

            CreateMap<MedicalNoteCreateDTO, MedicalNote>();
            CreateMap<MedicalNoteUpdateDTO, MedicalNote>();
            
            // MedicalNotesDTO is used specifically for adding notes
            CreateMap<MedicalNotesDTO, MedicalNote>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.MedicalRecordId, opt => opt.MapFrom(src => src.MedicalRecordId));
        }
    }
}
