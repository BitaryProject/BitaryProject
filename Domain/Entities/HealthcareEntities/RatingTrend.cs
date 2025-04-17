using System;

namespace Domain.Entities.HealthcareEntities
{
    public class RatingTrend : BaseEntity<Guid>
    {
        public string EntityType { get; set; } // "Doctor" or "Clinic"
        public Guid EntityId { get; set; } // DoctorId or ClinicId
        public DateTime Date { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
} 
