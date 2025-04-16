using Domain.Entities;
using System;

namespace Domain.Entities.HealthcareEntities
{
    public class PrescriptionMedicationItem : BaseEntity<Guid>
    {
        public Guid PrescriptionId { get; set; }
        public Guid MedicationId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
        
        // Navigation properties
        public Prescription Prescription { get; set; }
        public Medication Medication { get; set; }
    }
} 