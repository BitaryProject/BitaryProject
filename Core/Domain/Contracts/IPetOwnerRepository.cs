using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPetOwnerRepository : IGenericRepository<PetOwner, Guid>
    {
        Task<PetOwner> GetPetOwnerByUserIdAsync(string userId);
        Task<(IEnumerable<PetOwner> PetOwners, int TotalCount)> GetPagedPetOwnersAsync(Specifications<PetOwner> specifications, int pageIndex, int pageSize);
    }
} 