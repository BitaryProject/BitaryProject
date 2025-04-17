using System;
using System.Collections.Generic;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class MedicalRecord : BaseEntity<Guid>
    {
        public MedicalRecord()
        {
            MedicalNotes = new List<MedicalNote>();
        }
        
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public string Symptoms { get; set; }
        public string LabResults { get; set; }
        public string Medications { get; set; }
        
        // Foreign keys
        public Guid PetProfileId { get; set; }
        public Guid DoctorId { get; set; }
        
        // Navigation properties
        public PetProfile PetProfile { get; set; }
        public Doctor Doctor { get; set; }
        
        // Collection navigation property
        public ICollection<MedicalNote> MedicalNotes { get; set; }
    }
} 
