using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class DoctorRatingDTO
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public Guid PetOwnerId { get; set; }
        public string PetOwnerName { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
        public bool IsFlagged { get; set; }
        public string FlagReason { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class DoctorRatingCreateDTO
    {
        public Guid DoctorId { get; set; }
        public Guid PetOwnerId { get; set; }
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
    }

    public class DoctorRatingUpdateDTO
    {
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
    }

    public class DoctorRatingSummaryDTO
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public DateTime LastRatingDate { get; set; }
        
        // Added to support RatingService
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
    }
} 