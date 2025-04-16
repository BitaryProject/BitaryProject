using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class PetProfile : BaseEntity<Guid>
    {
        public PetProfile()
        {
            Appointments = new List<Appointment>();
            MedicalRecords = new List<MedicalRecord>();
        }

        public string PetName { get; set; }
        public int Age { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        
        // Foreign key
        public Guid OwnerId { get; set; }
        
        // Navigation properties
        public PetOwner Owner { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
} 