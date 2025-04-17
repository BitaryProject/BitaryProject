using Core.Common.Specifications;

using Core.Domain.Contracts;

using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class PetOwnerRepository : GenericRepository<PetOwner, Guid>, IPetOwnerRepository
    {
        private readonly StoreContext _storeContext;

        public PetOwnerRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        public async Task<PetOwner> GetByUserIdAsync(string userId)
        {
            return await _storeContext.PetOwners
                .Include(p => p.PetProfiles)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<(IEnumerable<PetOwner> PetOwners, int TotalCount)> GetPagedPetOwnersAsync(Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<PetOwner> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }
        
        public async Task<IEnumerable<PetOwner>> SearchPetOwnersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _storeContext.PetOwners.Take(10).ToListAsync();
                
            return await _storeContext.PetOwners
                .Where(p => p.FirstName.Contains(searchTerm) || 
                           p.LastName.Contains(searchTerm) || 
                           p.Email.Contains(searchTerm) ||
                           p.Phone.Contains(searchTerm))
                .ToListAsync();
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








