using System;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class Appointment : BaseEntity<Guid>
    {
        public DateTime AppointmentDateTime { get; set; }
        public DateTime AppointmentTime { get; set; } // Adding this property for backward compatibility
        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30); // Fixed at 30 minutes
        public int DurationMinutes { get; set; } = 30; // Adding this property for backward compatibility
        public AppointmentStatus Status { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        
        // Foreign keys
        public Guid PetProfileId { get; set; }
        public Guid PetId { get; set; } // Adding this property for backward compatibility
        public Guid ClinicId { get; set; }
        public Guid DoctorId { get; set; }
        
        // Navigation properties
        public PetProfile PetProfile { get; set; }
        public PetProfile Pet { get; set; } // Adding this property for backward compatibility
        public Clinic Clinic { get; set; }
        public Doctor Doctor { get; set; }
    }
    
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
} 