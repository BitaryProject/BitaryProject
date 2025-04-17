using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class PetOwnerDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public ICollection<PetDTO> Pets { get; set; } = new List<PetDTO>();
    }

    public class PetOwnerCreateUpdateDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }

    public class PetOwnerUpdateDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
} 