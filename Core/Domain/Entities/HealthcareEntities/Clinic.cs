using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class Clinic : BaseEntity<Guid>
    {
        public Clinic()
        {
            Doctors = new List<Doctor>();
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactDetails { get; set; }
        
        // Navigation properties
        public ICollection<Doctor> Doctors { get; set; }
    }
} 