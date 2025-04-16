using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Entities.HealthcareEntities
{
    public class PetOwner : BaseEntity<Guid>
    {
        public PetOwner()
        {
            PetProfiles = new List<PetProfile>();
        }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        
        // Navigation properties
        public ICollection<PetProfile> PetProfiles { get; set; }
        
        // Alias for backward compatibility
        public ICollection<PetProfile> Pets { get => PetProfiles; set => PetProfiles = value; }
        
        // User relationship
        public string UserId { get; set; } // Reference to the application user
    }
} 