using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class AppointmentDTO
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        
        public Guid PetProfileId { get; set; }
        public string PetName { get; set; }
        
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; }
        
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        
        // Follow-up appointment reference
        public Guid? FollowUpToAppointmentId { get; set; }
    }

    public class AppointmentCreateDTO
    {
        [Required]
        public DateTime AppointmentDateTime { get; set; }
        
        public TimeSpan? Duration { get; set; } = TimeSpan.FromHours(1);
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; }
        
        [Required]
        public Guid PetProfileId { get; set; }
        
        [Required]
        public Guid ClinicId { get; set; }
        
        [Required]
        public Guid DoctorId { get; set; }
        
        public Guid? FollowUpToAppointmentId { get; set; }
    }

    public class AppointmentUpdateDTO
    {
        public DateTime AppointmentDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; }
        
        [StringLength(500)]
        public string Reason { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; }
    }

    public class AppointmentCancellationDTO
    {
        [Required]
        [StringLength(500)]
        public string CancellationReason { get; set; }
    }
}