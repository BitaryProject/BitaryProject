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
    public enum Gender : byte
    {
        male = 1,
        female = 2,
        m = 1,
        f = 2
    }

    public class UserInformationDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(10)]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public AddressDTO Address { get; set; } = null!;

        public string? PhoneNumber { get; set; }
    }
}

