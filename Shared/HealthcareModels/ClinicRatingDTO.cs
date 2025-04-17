using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class ClinicRatingDTO
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public Guid PetOwnerId { get; set; }
        public string PetOwnerName { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
        public bool IsFlagged { get; set; }
        public string FlagReason { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ClinicRatingCreateDTO
    {
        public Guid ClinicId { get; set; }
        public Guid PetOwnerId { get; set; }
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
    }

    public class ClinicRatingUpdateDTO
    {
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
    }

    public class ClinicRatingSummaryDTO
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string ClinicLocation { get; set; } = string.Empty;
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