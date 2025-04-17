using Core.Domain.Entities;
using System;

namespace Core.Domain.Entities.HealthcareEntities
{
    // Rating Base
    public abstract class Rating : BaseEntity<Guid>
    {
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public Guid PetOwnerId { get; set; }
        
        // Navigation property
        public PetOwner PetOwner { get; set; }
    }

    // Doctor Rating
    public class DoctorRatingBase : Rating
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }

    // Clinic Rating
    public class ClinicRatingBase : Rating
    {
        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
} 
