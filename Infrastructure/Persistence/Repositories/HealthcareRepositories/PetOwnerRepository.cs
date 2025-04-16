using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Repositories.HealthcareRepositories
{
    public class PetOwnerRepository : GenericRepository<PetOwner, Guid>, IPetOwnerRepository
    {
        public PetOwnerRepository(StoreContext context) : base(context)
        {
        }

        public async Task<PetOwner> GetPetOwnerByUserIdAsync(string userId)
        {
            return await _context.PetOwners
                .Include(p => p.PetProfiles)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<(IEnumerable<PetOwner> PetOwners, int TotalCount)> GetPagedPetOwnersAsync(Specifications<PetOwner> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }
    }
} 