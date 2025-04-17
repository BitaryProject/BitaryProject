using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Core.Domain.Entities.SecurityEntities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class PetOwner : BaseEntity<Guid>
    {
        public PetOwner()
        {
            Pets = new List<PetProfile>();
            DoctorRatings = new List<DoctorRating>();
            ClinicRatings = new List<ClinicRating>();
            Appointments = new List<Appointment>();
        }

        public string UserId { get; set; }
        public User User { get; set; }
        
        public string Address { get; set; }
        public string PreferredContactMethod { get; set; }
        
        // Properties for easier mapping
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        
        // Navigation properties
        public ICollection<PetProfile> Pets { get; set; }
        public ICollection<DoctorRating> DoctorRatings { get; set; }
        public ICollection<ClinicRating> ClinicRatings { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
} 

