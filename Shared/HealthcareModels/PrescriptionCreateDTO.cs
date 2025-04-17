using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for creating a new Prescription
    /// </summary>
    public class PrescriptionCreateDTO
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
        /// Prescription issue date
        /// </summary>
        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Prescription expiry date
        /// </summary>
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Notes about the prescription
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// List of medications in this prescription
        /// </summary>
        [Required]
        public List<PrescriptionMedicationCreateDTO> Medications { get; set; } = new List<PrescriptionMedicationCreateDTO>();
    }
    
    /// <summary>
    /// Data transfer object for creating prescription medications
    /// </summary>
    public class PrescriptionMedicationCreateDTO
    {
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
        public string? Duration { get; set; }
        
        /// <summary>
        /// Special instructions for taking the medication
        /// </summary>
        public string? Instructions { get; set; }
    }
} 