using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPetProfileRepository : IGenericRepository<PetProfile, Guid>
    {
        Task<IEnumerable<PetProfile>> GetPetProfilesByOwnerIdAsync(Guid ownerId);
        Task<(IEnumerable<PetProfile> PetProfiles, int TotalCount)> GetPagedPetProfilesAsync(Specifications<PetProfile> specifications, int pageIndex, int pageSize);
    }
} 