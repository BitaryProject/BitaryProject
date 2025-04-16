using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for Clinic entity
    /// </summary>
    public class ClinicDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactDetails { get; set; }
        
        // Related data
        public ICollection<DoctorDTO> Doctors { get; set; }
    }
    
    // For creating or updating a clinic
    public record ClinicCreateUpdateDTO
    {
        public string Name { get; init; }
        public string Address { get; init; }
        public string ContactDetails { get; init; }
    }
} 