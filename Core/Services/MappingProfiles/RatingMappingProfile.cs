using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;

namespace Core.Services.MappingProfiles
{
    public class RatingMappingProfile : Profile
    {
        public RatingMappingProfile()
        {
            // Doctor Rating mappings
            CreateMap<DoctorRating, DoctorRatingDTO>()
                .ForMember(dest => dest.DoctorName, opt => 
                    opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : string.Empty))
                .ForMember(dest => dest.PetOwnerName, opt => 
                    opt.MapFrom(src => src.PetOwner != null ? $"{src.PetOwner.FirstName} {src.PetOwner.LastName}" : string.Empty));
            
            CreateMap<DoctorRatingCreateDTO, DoctorRating>();
            CreateMap<DoctorRatingUpdateDTO, DoctorRating>();
            
            // Clinic Rating mappings
            CreateMap<ClinicRating, ClinicRatingDTO>()
                .ForMember(dest => dest.ClinicName, opt => 
                    opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : string.Empty))
                .ForMember(dest => dest.PetOwnerName, opt => 
                    opt.MapFrom(src => src.PetOwner != null ? $"{src.PetOwner.FirstName} {src.PetOwner.LastName}" : string.Empty));
            
            CreateMap<ClinicRatingCreateDTO, ClinicRating>();
            CreateMap<ClinicRatingUpdateDTO, ClinicRating>();
        }
    }
} 
