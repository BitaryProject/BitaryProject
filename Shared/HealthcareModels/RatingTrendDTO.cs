using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class RatingTrendDTO
    {
        public DateTime Date { get; set; } // Typically aggregated by month or day
        public DateTime Period { get; set; } // Used by some services for aggregation
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int Count { get; set; }
    }

    public class RatingAnalyticsDTO
    {
        public double OverallAverage { get; set; }
        public int TotalRatings { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<RatingTrendDTO> Trends { get; set; }
    }
} 