using AutoMapper;
using Core.Domain.Entities.SecurityEntities;
using Domain.Exceptions;
using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.MappingProfiles
{
    public class SecurityProfile : Profile
    {
        public SecurityProfile()
        {
            // User registration mapping
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            // User to UserResultDTO mapping
            CreateMap<User, UserResultDTO>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Token, opt => opt.Ignore()) // Token to be provided separately
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles to be provided separately

            // Login mapping
            CreateMap<LoginDTO, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // User information mapping
            CreateMap<User, UserInformationDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            // Update user information mapping
            CreateMap<UserInformationDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.LastName)))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PhoneNumber, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.PhoneNumber)))
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Address, opt => opt.Ignore()); // Handled separately

            // JWT claims mapping
            CreateMap<User, JwtClaimsDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            // Email verification mapping
            CreateMap<EmailVerificationDTO, EmailVerificationRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.OTP, opt => opt.MapFrom(src => src.OTP));

            // Password reset mapping
            CreateMap<PasswordResetDTO, PasswordResetRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.NewPassword, opt => opt.MapFrom(src => src.NewPassword));

            // API response mappings
            CreateMap<ApiResponse<UserResultDTO>, ApiResponseDTO<UserResultDTO>>();
            CreateMap<ApiResponse<bool>, ApiResponseDTO<bool>>();
            CreateMap<ApiResponse<AddressDTO>, ApiResponseDTO<AddressDTO>>();

            // Error mappings
            CreateMap<Domain.Exceptions.ValidationException, ValidationErrorDTO>()
                .ForMember(dest => dest.Errors, opt => opt.MapFrom(src => src.Errors));
            CreateMap<UnauthorizedAccessException, ErrorDTO>();
            CreateMap<Domain.Exceptions.UserNotFoundException, ErrorDTO>();
        }
    }
} 
