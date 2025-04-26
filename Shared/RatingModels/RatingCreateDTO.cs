using System.ComponentModel.DataAnnotations;

namespace Shared.RatingModels
{
    public record RatingCreateDTO
    {
        [Required(ErrorMessage = "Clinic ID is required")]
        public int ClinicId { get; init; }
        
        [Required(ErrorMessage = "Rating value is required")]
        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5")]
        public int RatingValue { get; init; }
        
        [MaxLength(500, ErrorMessage = "Comment cannot be longer than 500 characters")]
        public string Comment { get; init; }
    }
} 