using System;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class MedicalRecord : BaseEntity<Guid>
    {
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string AdditionalNotes { get; set; }
        public string Notes { get; set; }
        
        // Foreign keys
        public Guid PetProfileId { get; set; }
        public Guid PetId { get; set; }
        public Guid DoctorId { get; set; }
        
        // Navigation properties
        public PetProfile PetProfile { get; set; }
        public PetProfile Pet { get; set; }
        public Doctor Doctor { get; set; }
    }
} 