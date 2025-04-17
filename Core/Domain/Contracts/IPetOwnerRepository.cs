using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IPetOwnerRepository : IGenericRepository<PetOwner, Guid>
    {
        Task<PetOwner> GetPetOwnerByUserIdAsync(string userId);
        Task<(IEnumerable<PetOwner> PetOwners, int TotalCount)> GetPagedPetOwnersAsync(ISpecification<PetOwner> specifications, int pageIndex, int pageSize);
        Task<PetOwner> GetByUserIdAsync(string userId);
        Task<IEnumerable<PetOwner>> SearchPetOwnersAsync(string searchTerm);
    }
} 

