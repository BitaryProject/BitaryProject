using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Domain.Entities.HealthcareEntities;
using Domain.Entities.SecurityEntities;
using Shared.HealthcareModels;
using Shared.SecurityModels;

namespace Services.MappingProfiles
{
    public class HealthcareUserProfile : Profile
    {
        public HealthcareUserProfile()
        {
            // User to Doctor mapping
            CreateMap<User, Doctor>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => "General"))
                .ForMember(dest => dest.LicenseNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Bio, opt => opt.Ignore())
                .ForMember(dest => dest.Clinics, opt => opt.Ignore())
                .ForMember(dest => dest.Appointments, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
                .ForMember(dest => dest.Prescriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Ratings, opt => opt.Ignore());

            // User to PetOwner mapping
            CreateMap<User, PetOwner>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? "Please update phone number"))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => "Please update address"))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            // Doctor data transfer object mappings
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialization))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.ContactDetails))
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name));
            
            CreateMap<DoctorDTO, Doctor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialty))
                .ForMember(dest => dest.ContactDetails, opt => opt.MapFrom(src => src.Contact))
                .ForMember(dest => dest.Clinic, opt => opt.Ignore()); // Clinic needs to be set separately

            // PetOwner data transfer object mappings
            CreateMap<PetOwner, PetOwnerDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.Address));
            
            CreateMap<PetOwnerDTO, PetOwner>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.HomeAddress));

            // Clinic mapping (in context of user)
            CreateMap<Clinic, ClinicDTO>().ReverseMap();

            // UserRole mapping
            CreateMap<string, UserRoleDTO>() // Mapping from role name string
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src));

            // ClinicInfoDTO to ClinicInfo
            CreateMap<ClinicInfoDTO, ClinicInfo>();
            
            // User registration to User with clinic info
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ClinicInfo, opt => opt.MapFrom(src => src.ClinicInfo))
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore());
        }
    }
} 