using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IPetProfileRepository : IGenericRepository<PetProfile, Guid>
    {
        Task<IEnumerable<PetProfile>> GetPetProfilesByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<PetProfile>> GetPetProfilesBySpeciesAsync(string species);
        Task<IEnumerable<PetProfile>> GetPetProfilesByBreedAsync(string breed);
        Task<IEnumerable<PetProfile>> SearchPetProfilesAsync(string searchTerm);
        Task<(IEnumerable<PetProfile> Profiles, int TotalCount)> GetPagedPetProfilesAsync(ISpecification<PetProfile> specification, int pageIndex, int pageSize);
        Task<ICollection<Guid>> GetPetIdsByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<PetProfile>> GetPetsByOwnerIdAsync(Guid ownerId);
        Task<bool> IsPetOwnedByUserAsync(Guid petId, string userId);
        Task<(IEnumerable<PetProfile> Pets, int TotalCount)> GetPagedPetsAsync(ISpecification<PetProfile> specification, int pageIndex, int pageSize);
    }
} 

