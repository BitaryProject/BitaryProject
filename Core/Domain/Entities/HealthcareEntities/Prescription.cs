using System;
using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Entities.HealthcareEntities
{
    public class Prescription : BaseEntity<Guid>
    {
        public string PrescriptionNumber { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public string Notes { get; set; }
        
        // Foreign keys
        public Guid PetProfileId { get; set; }
        public Guid PetId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? MedicalRecordId { get; set; }
        
        // Navigation properties
        public PetProfile PetProfile { get; set; }
        public Doctor Doctor { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        
        // Collection navigation properties
        public ICollection<PrescriptionMedicationItem> MedicationItems { get; set; } = new List<PrescriptionMedicationItem>();
    }
} 