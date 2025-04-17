using System;
using System.Collections.Generic;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public enum PrescriptionStatus
    {
        Draft,
        Active,
        Completed,
        Cancelled
    }

    public class Prescription : BaseEntity<Guid>
    {
        public Prescription()
        {
            MedicationItems = new List<PrescriptionMedicationItem>();
            // Generate a unique prescription number when created
            PrescriptionNumber = $"RX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        
        public string PrescriptionNumber { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public string Instructions { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Draft;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Foreign keys
        public Guid PetProfileId { get; set; }
        public Guid DoctorId { get; set; }
        
        // Navigation properties
        public PetProfile PetProfile { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<PrescriptionMedicationItem> MedicationItems { get; set; }
    }
} 
