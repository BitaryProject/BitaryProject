using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class PetProfileDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string MicrochipNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }

    public class PetProfileCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Species { get; set; } = string.Empty;
        
        public string Breed { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        public string Color { get; set; } = string.Empty;
        
        public double Weight { get; set; }
        
        public string MicrochipNumber { get; set; } = string.Empty;
        
        public string? ProfilePictureUrl { get; set; }
        
        public string Notes { get; set; } = string.Empty;
    }

    public class PetProfileUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        
        public string Breed { get; set; } = string.Empty;
        
        public string Color { get; set; } = string.Empty;
        
        public double? Weight { get; set; }
        
        public string MicrochipNumber { get; set; } = string.Empty;
        
        public string? ProfilePictureUrl { get; set; }
        
        public string Notes { get; set; } = string.Empty;
    }
} 