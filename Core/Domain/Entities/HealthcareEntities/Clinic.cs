using System;
using System.Collections.Generic;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class Clinic : BaseEntity<Guid>
    {
        public Clinic()
        {
            Doctors = new List<Doctor>();
            Appointments = new List<Appointment>();
            Ratings = new List<ClinicRating>();
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // Navigation properties
        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<ClinicRating> Ratings { get; set; }
    }
} 
