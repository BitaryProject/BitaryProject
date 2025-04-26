using AutoMapper;
using Domain.Entities.ClinicEntities;
using Shared.RatingModels;
using System;

namespace Services.MappingProfiles
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            // Entity to DTO mapping
            CreateMap<Rating, RatingDTO>()
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.ClinicName : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => "User")); // Note: We'll need to fetch username from a service

            // DTO to Entity mapping (for creation)
            CreateMap<RatingCreateDTO, Rating>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Clinic, opt => opt.Ignore());

            // DTO to Entity mapping (for updates)
            CreateMap<RatingUpdateDTO, Rating>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ClinicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Clinic, opt => opt.Ignore());
        }
    }
} 