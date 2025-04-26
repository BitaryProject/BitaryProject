using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.RatingModels
{
    public record RatingDTO
    {
        public int Id { get; init; }
        
        public string UserId { get; init; }
        public string UserName { get; init; }
        
        public int ClinicId { get; init; }
        public string ClinicName { get; init; }
        
        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5")]
        public int RatingValue { get; init; }
        
        [MaxLength(500, ErrorMessage = "Comment cannot be longer than 500 characters")]
        public string Comment { get; init; }
        
        public DateTime CreatedAt { get; init; }
    }
} 