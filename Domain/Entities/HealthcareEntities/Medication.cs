using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Entities.HealthcareEntities
{
    public class Medication : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool RequiresPrescription { get; set; }
        
        // Navigation properties
        public ICollection<PrescriptionMedicationItem> PrescriptionItems { get; set; } = new List<PrescriptionMedicationItem>();
    }
} 