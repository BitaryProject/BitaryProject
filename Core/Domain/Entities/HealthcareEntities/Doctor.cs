using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class Doctor : BaseEntity<Guid>
    {
        public Doctor()
        {
            Appointments = new List<Appointment>();
            MedicalRecords = new List<MedicalRecord>();
            Prescriptions = new List<Prescription>();
        }

        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string ContactDetails { get; set; }
        
        // Foreign key
        public Guid ClinicId { get; set; }
        
        // Navigation properties
        public Clinic Clinic { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        
        // User relationship
        public string UserId { get; set; } // Reference to the application user
    }
} 