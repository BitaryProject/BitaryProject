using Domain.Entities.SecurityEntities;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SecurityModels
{
    public class UserInformationDTO
    {
        [Required]
        public string FirstName { get; set; }

        [MaxLength(10)]
        [Required]
        public string LastName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Role UserRole { get; set; }

        [Required]
        public AddressDTO Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}

