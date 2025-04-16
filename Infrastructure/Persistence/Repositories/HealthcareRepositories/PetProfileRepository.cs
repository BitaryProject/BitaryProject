using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories.HealthcareRepositories
{
    public class PetProfileRepository : GenericRepository<PetProfile, Guid>, IPetProfileRepository
    {
        public PetProfileRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PetProfile>> GetPetProfilesByOwnerIdAsync(Guid ownerId)
        {
            return await _context.PetProfiles
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<(IEnumerable<PetProfile> PetProfiles, int TotalCount)> GetPagedPetProfilesAsync(Specifications<PetProfile> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }
    }
} 