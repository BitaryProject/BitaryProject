using AutoMapper;
using Domain.Entities.AppointmentEntities;
using Shared.AppointmentModels;
using System;

namespace Services.MappingProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            // Entity to DTO mapping
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetProfile != null ? src.PetProfile.PetName : null))
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.ClinicName : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

            // DTO to Entity mapping (for creation)
            CreateMap<AppointmentDTO, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PetProfile, opt => opt.Ignore())
                .ForMember(dest => dest.Clinic, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecordId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 0 ? AppointmentStatus.Pending : src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt == default ? DateTime.UtcNow : src.CreatedAt));
        }
    }
} 