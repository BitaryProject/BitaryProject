using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for Doctor entity
    /// </summary>
    public class DoctorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Contact { get; set; }
        public string ClinicName { get; set; }
        public Guid ClinicId { get; set; }
        public string UserId { get; set; }
        
        // Related data - can be included or excluded as needed
        public ICollection<AppointmentDTO> Appointments { get; init; }
    }
    
    // For creating or updating a doctor
    public record DoctorCreateUpdateDTO
    {
        public string FullName { get; init; }
        public string Specialization { get; init; }
        public string ContactDetails { get; init; }
        public Guid ClinicId { get; init; }
        public string UserId { get; init; }
    }
    
    // For registration as a doctor
    public record DoctorRegistrationDTO
    {
        public string FullName { get; init; }
        public string Specialization { get; init; }
        public string ContactDetails { get; init; }
        
        // Clinic information
        public string ClinicName { get; init; }
        public string ClinicAddress { get; init; }
        public string ClinicContactDetails { get; init; }
        
        // User registration information
        public string Email { get; init; }
        public string Password { get; init; }
    }
} 