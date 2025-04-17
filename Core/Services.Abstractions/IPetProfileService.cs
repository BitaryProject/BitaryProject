using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IPetProfileService
    {
        // Get pet profile by ID
        Task<PetProfileDTO> GetPetProfileByIdAsync(Guid id);
        
        // Get all pet profiles for an owner
        Task<IEnumerable<PetProfileDTO>> GetPetProfilesByOwnerIdAsync(Guid ownerId);
        
        // Create a new pet profile
        Task<PetProfileDTO> CreatePetProfileAsync(Guid ownerId, PetProfileCreateDTO model);
        
        // Update a pet profile
        Task<PetProfileDTO> UpdatePetProfileAsync(Guid id, PetProfileUpdateDTO model);
        
        // Delete a pet profile
        Task<bool> DeletePetProfileAsync(Guid id);
    }
} 
