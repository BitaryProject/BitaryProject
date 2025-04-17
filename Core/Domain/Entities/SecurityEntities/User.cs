global using Microsoft.AspNetCore.Identity;
//using Core.Domain.Entities.DoctorEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.SecurityEntities
{
    //public enum UserRole :byte
    //{
    //    PetOwner =1,
    //    Doctor=2,
    //}
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
        public Gender Gender { get; set; }
        //public UserRole UserRole { get; set; }
        //public Doctor DoctorProfile { get; set; }

        // Navigation property for Address
        public virtual UserAddress Address { get; set; }
        
        // Transient property for clinic info during registration (not stored in DB)
        [NotMapped]
        public ClinicInfo ClinicInfo { get; set; }
    }
    
    public class UserAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        
        // Navigation property
        public string UserId { get; set; }
        public User User { get; set; }
    }
}

