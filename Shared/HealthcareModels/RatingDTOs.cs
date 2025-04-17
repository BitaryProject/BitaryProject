using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class RatingDTO
    {
        public Guid Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime RatingDate { get; set; }
        public string PetOwnerName { get; set; } = string.Empty;
    }

    public class RatingCreateDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Stars { get; set; }
        
        public string Comment { get; set; } = string.Empty;
    }

    // DTO for top-rated entities
    public class DoctorWithRatingDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
    }
    
    public class ClinicWithRatingDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }
} 