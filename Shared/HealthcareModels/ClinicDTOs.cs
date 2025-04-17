using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    public class ClinicDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public ICollection<DoctorDTO> Doctors { get; set; }
    }

    public class ClinicCreateDTO
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public string Description { get; set; }
        
        public string Website { get; set; }
        
        public string ImageUrl { get; set; }
        
        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
    }

    public class ClinicUpdateDTO
    {
        public string Name { get; set; }
        
        public string Address { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public string Description { get; set; }
        
        public string Website { get; set; }
        
        public string ImageUrl { get; set; }
        
        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
    }
} 