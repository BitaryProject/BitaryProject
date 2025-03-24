global using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SecurityEntities
{
    public enum Gender : byte
    {
        male = 1,
        female = 2,
        m = 1,
        f = 2
    }
    public class User :IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public Address Address { get; set; }
        public Gender Gender { get; set; }

    }
}
