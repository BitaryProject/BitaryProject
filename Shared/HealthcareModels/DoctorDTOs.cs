using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class DoctorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public ICollection<ClinicDTO> Clinics { get; set; }
    }

    public class DoctorProfileUpdateDTO
    {
        [Required]
        public string Name { get; set; }
        
        public string Phone { get; set; }
        
        [Required]
        public string Specialization { get; set; }
        
        public string ProfilePictureUrl { get; set; }
        
        public string Bio { get; set; }
    }

    public class DoctorCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Specialization { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Bio { get; set; }
    }

    public class DoctorUpdateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Specialization { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Bio { get; set; }
    }
} 