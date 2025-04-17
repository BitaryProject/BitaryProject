using System;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    /// <summary>
    /// Represents the trend of ratings over time for a doctor or clinic
    /// </summary>
    public class RatingTrend : BaseEntity<Guid>
    {
        /// <summary>
        /// The type of entity this trend is for (Doctor or Clinic)
        /// </summary>
        public string EntityType { get; set; } = string.Empty;
        
        /// <summary>
        /// The ID of the entity (DoctorId or ClinicId)
        /// </summary>
        public Guid EntityId { get; set; }
        
        /// <summary>
        /// The date of this rating trend entry
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// The average rating as of this date
        /// </summary>
        public double AverageRating { get; set; }
        
        /// <summary>
        /// The total number of ratings as of this date
        /// </summary>
        public int TotalRatings { get; set; }
    }
} 