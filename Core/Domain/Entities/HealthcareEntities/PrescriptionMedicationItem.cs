using Core.Domain.Entities;
using System;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class PrescriptionMedicationItem : BaseEntity<Guid>
    {
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        
        // Foreign keys
        public Guid PrescriptionId { get; set; }
        public Guid MedicationId { get; set; }
        
        // Navigation properties
        public Prescription Prescription { get; set; }
        public Medication Medication { get; set; }
    }
} 
