using System;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }

    public class Appointment : BaseEntity<Guid>
    {
        public Guid PetProfileId { get; set; }
        public PetProfile PetProfile { get; set; }
        
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        
        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        
        public DateTime AppointmentDateTime { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
        public string Reason { get; set; }
        public string Notes { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        
        // Follow-up appointment reference
        public Guid? FollowUpToAppointmentId { get; set; }
        public Appointment FollowUpToAppointment { get; set; }
    }
} 
