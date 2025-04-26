using System;
using Domain.Entities.SecurityEntities;

namespace Domain.Entities.ClinicEntities
{
    public class Rating : BaseEntity<int>
    {
        public Rating()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public Rating(string userId, int clinicId, int ratingValue, string comment)
        {
            UserId = userId;
            ClinicId = clinicId;
            RatingValue = ratingValue;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }

        // Rating value (1-5)
        public int RatingValue { get; set; }
        
        // Optional comment/review
        public string Comment { get; set; }
        
        // Creation timestamp
        public DateTime CreatedAt { get; set; }
        
        // Relationships
        public string UserId { get; set; }
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
} 