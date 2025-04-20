using Domain.Entities.SecurityEntities;
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
        public string FirstName { get; init; }    

        [Required(ErrorMessage = "LastName Is Required")]
        public string LastName { get; init; }

        public string DisplayName => $"{FirstName} {LastName}";
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; init; }

        [Required(ErrorMessage = "UserName Is Required")]
        public string UserName { get; init; }

        [Phone]
        public string PhoneNumber { get; init; }
        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Role UserRole { get; set; }

    }
}
