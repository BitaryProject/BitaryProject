using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SecurityModels
{
    public record UserRegisterDTO
    {
        [Required(ErrorMessage = "FirstName Is Required")]
        public string FirstName { get; init; } = string.Empty;

        [Required(ErrorMessage = "LastName Is Required")]
        public string LastName { get; init; } = string.Empty;

        public string DisplayName => $"{FirstName} {LastName}";
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; init; } = string.Empty;

        [Required(ErrorMessage = "UserName Is Required")]
        public string UserName { get; init; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; init; }
        [Required]
        public Gender Gender { get; set; }

        // Property for role assignment
        public string? UserType { get; init; }

        // Clinic information for doctor registration
        public ClinicInfoDTO? ClinicInfo { get; init; }

        //[Required]
        //public UserRole UserRole { get; set; }

    }

    public class ClinicInfoDTO
    {
        [Required]
        public string Name { get; init; } = string.Empty;
        
        [Required]
        public string Address { get; init; } = string.Empty;
        
        public string? Phone { get; init; }
        
        public string? Email { get; init; }
        
        public string? Description { get; init; }
    }
}
