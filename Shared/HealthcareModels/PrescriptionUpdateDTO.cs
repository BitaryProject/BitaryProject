using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for updating an existing Prescription
    /// </summary>
    public class PrescriptionUpdateDTO
    {
        /// <summary>
        /// Optional medical record ID associated with this prescription
        /// </summary>
        public Guid? MedicalRecordId { get; set; }
        
        /// <summary>
        /// Doctor who issued the prescription
        /// </summary>
        [Required]
        public Guid DoctorId { get; set; }
        
        /// <summary>
        /// Patient for whom the prescription is issued
        /// </summary>
        [Required]
        public Guid PatientId { get; set; }
        
        /// <summary>
        /// Pet profile ID
        /// </summary>
        [Required]
        public Guid PetProfileId { get; set; }
        
        /// <summary>
        /// Prescription expiry date
        /// </summary>
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Start date for the prescription
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// End date for the prescription
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Medication information
        /// </summary>
        [Required]
        public string Medication { get; set; } = string.Empty;
        
        /// <summary>
        /// Dosage instructions
        /// </summary>
        [Required]
        public string Dosage { get; set; } = string.Empty;
        
        /// <summary>
        /// Usage instructions
        /// </summary>
        public string Instructions { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional notes
        /// </summary>
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Status of the prescription
        /// </summary>
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// List of medications in this prescription
        /// </summary>
        public List<PrescriptionMedicationUpdateDTO> Medications { get; set; } = new List<PrescriptionMedicationUpdateDTO>();
    }
    
    /// <summary>
    /// Data transfer object for updating prescription medications
    /// </summary>
    public class PrescriptionMedicationUpdateDTO
    {
        /// <summary>
        /// ID of the medication item to update (required for updates)
        /// </summary>
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Name of the medication
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Dosage instructions
        /// </summary>
        [Required]
        public string Dosage { get; set; } = string.Empty;
        
        /// <summary>
        /// Frequency of medication (e.g., "twice daily")
        /// </summary>
        [Required]
        public string Frequency { get; set; } = string.Empty;
        
        /// <summary>
        /// Duration of medication course
        /// </summary>
        public string Duration { get; set; } = string.Empty;
        
        /// <summary>
        /// Special instructions for taking the medication
        /// </summary>
        public string Instructions { get; set; } = string.Empty;
        
        /// <summary>
        /// Quantity of medication
        /// </summary>
        public int Quantity { get; set; }
    }
} 