using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public record PetProfileDTO
    {
        public Guid Id { get; init; }
        public string PetName { get; init; }
        public int Age { get; init; }
        public string Species { get; init; }
        public string Breed { get; init; }
        public Guid OwnerId { get; init; }
        public string OwnerName { get; init; } // For display purposes
        
        // Related data - can be included or excluded as needed
        public ICollection<AppointmentDTO> Appointments { get; init; }
        public ICollection<MedicalRecordDTO> MedicalRecords { get; init; }
    }
    
    // For creating or updating a pet profile
    public record PetProfileCreateUpdateDTO
    {
        public string PetName { get; init; }
        public int Age { get; init; }
        public string Species { get; init; }
        public string Breed { get; init; }
        public Guid OwnerId { get; init; }
    }
} 