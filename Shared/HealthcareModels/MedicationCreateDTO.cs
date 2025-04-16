using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for creating a new Medication
    /// </summary>
    public class MedicationCreateDTO
    {
        /// <summary>
        /// Name of the medication
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the medication
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Dosage form of the medication (e.g., tablet, liquid, etc.)
        /// </summary>
        [Required]
        public string DosageForm { get; set; }
        
        /// <summary>
        /// Manufacturer of the medication
        /// </summary>
        [Required]
        public string Manufacturer { get; set; }
        
        /// <summary>
        /// Price of the medication
        /// </summary>
        [Required]
        public decimal Price { get; set; }
        
        /// <summary>
        /// Category of the medication
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Whether the medication requires a prescription
        /// </summary>
        public bool RequiresPrescription { get; set; }
    }
} 