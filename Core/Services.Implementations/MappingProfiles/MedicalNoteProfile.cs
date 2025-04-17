using System;
using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;

namespace Core.Services.Implementations.MappingProfiles
{
    public class MedicalNoteProfile : Profile
    {
        public MedicalNoteProfile()
        {
            CreateMap<MedicalNote, MedicalNoteDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => 
                    src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : "Unknown"));

            CreateMap<MedicalNoteCreateDTO, MedicalNote>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<MedicalNotesDTO, MedicalNote>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp));
        }
    }
} 