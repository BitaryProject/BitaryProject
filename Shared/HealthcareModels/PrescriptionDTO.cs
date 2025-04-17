using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for Prescription entity
    /// </summary>
    public class PrescriptionDTO
    {
        public Guid Id { get; set; }
        public string PrescriptionNumber { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        
        // Related entity information
        public string DoctorName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public Guid PetId { get; set; }
        public string PetOwnerName { get; set; } = string.Empty;
        
        // Medication items
        public ICollection<PrescriptionMedicationItemDTO> MedicationItems { get; set; } = new List<PrescriptionMedicationItemDTO>();
    }
    
    /// <summary>
    /// Data transfer object for prescription medication item
    /// </summary>
    public class PrescriptionMedicationItemDTO
    {
        public Guid Id { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }
    
    // For creating or updating a prescription
    public record PrescriptionCreateUpdateDTO
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string Medication { get; init; } = string.Empty;
        public string Dosage { get; init; } = string.Empty;
        public string Instructions { get; init; } = string.Empty;
        public string Notes { get; init; } = string.Empty;
        public Guid PetProfileId { get; init; }
        public Guid DoctorId { get; init; }
        public Guid? MedicalRecordId { get; init; }
    }
} 