using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for PetOwner entity
    /// </summary>
    public class PetOwnerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string HomeAddress { get; set; }
        public string UserId { get; set; }
        
        // Related data
        public ICollection<PetDTO> Pets { get; set; }
    }
    
    // For creating or updating a pet owner
    public record PetOwnerCreateUpdateDTO
    {
        public string FullName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public string UserId { get; init; }
    }
    
    // For registration as a pet owner
    public record PetOwnerRegistrationDTO
    {
        public string FullName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        
        // User registration information
        public string Password { get; init; }
    }
} 