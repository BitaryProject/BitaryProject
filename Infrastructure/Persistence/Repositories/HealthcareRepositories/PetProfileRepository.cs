using Core.Common.Specifications;

using Core.Domain.Contracts;

using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class PetProfileRepository : GenericRepository<PetProfile, Guid>, IPetProfileRepository
    {
        private readonly StoreContext _storeContext;
        
        public PetProfileRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        public async Task<IEnumerable<PetProfile>> GetPetsByOwnerIdAsync(Guid ownerId)
        {
            return await _storeContext.PetProfiles
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Guid>> GetPetIdsByOwnerIdAsync(Guid ownerId)
        {
            return await _storeContext.PetProfiles
                .Where(p => p.OwnerId == ownerId)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<(IEnumerable<PetProfile> PetProfiles, int TotalCount)> GetPagedPetsAsync(Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<PetProfile> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }

        public async Task<IEnumerable<PetProfile>> GetPetProfilesBySpeciesAsync(string species)
        {
            return await _storeContext.PetProfiles
                .Include(p => p.Owner)
                .Where(p => p.Species == species)
                .ToListAsync();
        }

        public async Task<IEnumerable<PetProfile>> GetPetProfilesByBreedAsync(string breed)
        {
            return await _storeContext.PetProfiles
                .Include(p => p.Owner)
                .Where(p => p.Breed == breed)
                .ToListAsync();
        }

        public async Task<IEnumerable<PetProfile>> SearchPetProfilesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _storeContext.PetProfiles.Take(10).ToListAsync();
                
            return await _storeContext.PetProfiles
                .Include(p => p.Owner)
                .Where(p => p.Name.Contains(searchTerm) || 
                           p.Species.Contains(searchTerm) || 
                           p.Breed.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<PetProfile>> GetPagedPetProfilesAsync(Core.Common.Specifications.ISpecification<PetProfile> spec, int pageIndex, int pageSize)
        {
            // Implement pagination logic
            return await Task.FromResult(Enumerable.Empty<PetProfile>());
        }

        public async Task<bool> IsPetOwnedByUserAsync(Guid petId, string userId)
        {
            var pet = await _storeContext.PetProfiles
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == petId);
                
            if (pet == null)
                return false;
                
            return pet.Owner.UserId == userId;
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








