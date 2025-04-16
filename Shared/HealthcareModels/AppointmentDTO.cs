using System;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for appointment
    /// </summary>
    public class AppointmentDTO
    {
        public Guid Id { get; set; }
        public DateTime AppointmentTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        
        // Related entity information
        public string DoctorName { get; set; }
        public Guid DoctorId { get; set; }
        public string PetName { get; set; }
        public Guid PetId { get; set; }
        public string PetOwnerName { get; set; }
        public string ClinicName { get; set; }
        public Guid ClinicId { get; set; }
    }
    
    /// <summary>
    /// DTO for creating an appointment
    /// </summary>
    public class AppointmentCreateDTO
    {
        public DateTime AppointmentTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public string Reason { get; set; }
        public string Notes { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PetId { get; set; }
        public Guid ClinicId { get; set; }
    }
    
    /// <summary>
    /// DTO for updating an appointment
    /// </summary>
    public class AppointmentUpdateDTO
    {
        public DateTime AppointmentTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
    }
} 