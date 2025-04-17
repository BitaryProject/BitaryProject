using System;

namespace Domain.Entities.HealthcareEntities
{
    public class ClinicRating : BaseEntity<Guid>
    {
        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public Guid PetOwnerId { get; set; }
        public PetOwner PetOwner { get; set; }
        public int Rating { get; set; } // 1-5 stars
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsFlagged { get; set; }
        public string FlagReason { get; set; }
        public Guid? AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
} 
