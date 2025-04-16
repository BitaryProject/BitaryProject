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
        public string PrescriptionNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
        public string Instructions { get; set; }
        
        // Related entity information
        public string DoctorName { get; set; }
        public Guid DoctorId { get; set; }
        public string PetName { get; set; }
        public Guid PetId { get; set; }
        public string PetOwnerName { get; set; }
        
        // Medication items
        public ICollection<PrescriptionMedicationItemDTO> MedicationItems { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for prescription medication item
    /// </summary>
    public class PrescriptionMedicationItemDTO
    {
        public Guid Id { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
    }
    
    // For creating or updating a prescription
    public record PrescriptionCreateUpdateDTO
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string Medication { get; init; }
        public string Dosage { get; init; }
        public string Instructions { get; init; }
        public string Notes { get; init; }
        public Guid PetProfileId { get; init; }
        public Guid DoctorId { get; init; }
        public Guid? MedicalRecordId { get; init; }
    }
} 