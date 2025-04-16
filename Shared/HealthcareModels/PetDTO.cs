using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for Pet entity
    /// </summary>
    public class PetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Color { get; set; }
        public decimal Weight { get; set; }
        public string MicrochipNumber { get; set; }
        public string Notes { get; set; }
        
        // Owner information
        public string OwnerName { get; set; }
        public Guid OwnerId { get; set; }
        
        // Related collections
        public ICollection<MedicalRecordDTO> RecentMedicalRecords { get; set; }
    }
    
    /// <summary>
    /// DTO for creating a pet
    /// </summary>
    public class PetCreateDTO
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Color { get; set; }
        public decimal Weight { get; set; }
        public string MicrochipNumber { get; set; }
        public string Notes { get; set; }
        public Guid OwnerId { get; set; }
    }
    
    /// <summary>
    /// DTO for updating a pet
    /// </summary>
    public class PetUpdateDTO
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }
        public decimal Weight { get; set; }
        public string MicrochipNumber { get; set; }
        public string Notes { get; set; }
    }
} 