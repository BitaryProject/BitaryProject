using System;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for medication
    /// </summary>
    public class MedicationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool RequiresPrescription { get; set; }
    }
} 