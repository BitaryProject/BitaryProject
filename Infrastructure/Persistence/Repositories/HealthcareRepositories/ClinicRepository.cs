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
    public class ClinicRepository : GenericRepository<Clinic, Guid>, IClinicRepository
    {
        private readonly StoreContext _storeContext;

        public ClinicRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        public async Task<IEnumerable<Clinic>> GetClinicsByLocationAsync(string city, string state)
        {
            return await _storeContext.Clinics
                .Include(c => c.Doctors)
                .Where(c => c.City.Contains(city) && c.State.Contains(state))
                .ToListAsync();
        }

        public async Task<IEnumerable<Clinic>> SearchClinicsAsync(string searchTerm)
        {
            searchTerm = searchTerm?.ToLower() ?? "";
            return await _storeContext.Clinics
                .Include(c => c.Doctors)
                .Where(c => c.Name.ToLower().Contains(searchTerm) || 
                            c.Address.ToLower().Contains(searchTerm) ||
                            c.City.ToLower().Contains(searchTerm) ||
                            c.State.ToLower().Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<Clinic> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }
        
        public async Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count)
        {
            return await _storeContext.Clinics
                .Include(c => c.Ratings)
                .OrderByDescending(c => c.Ratings.Average(r => r.Rating))
                .Take(count)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Clinic>> GetClinicsByStatusAsync(string status)
        {
            return await _storeContext.Clinics
                .Where(c => c.Status == status)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Clinic>> GetClinicsByDoctorIdAsync(Guid doctorId)
        {
            return await _storeContext.Clinics
                .Include(c => c.Doctors)
                .Where(c => c.Doctors.Any(d => d.Id == doctorId))
                .ToListAsync();
        }
        
        public async Task<IEnumerable<ClinicRating>> GetRatingsAsync(Guid clinicId)
        {
            return await _storeContext.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
        }
        
        public async Task<ClinicRating> GetRatingByIdAsync(Guid ratingId)
        {
            return await _storeContext.ClinicRatings
                .FirstOrDefaultAsync(r => r.Id == ratingId);
        }
        
        public async Task<ClinicRating> GetRatingByUserAsync(Guid clinicId, Guid userId)
        {
            return await _storeContext.ClinicRatings
                .FirstOrDefaultAsync(r => r.ClinicId == clinicId && r.UserId == userId);
        }
        
        public async Task<IEnumerable<ClinicRating>> GetRatingsByUserIdAsync(Guid userId)
        {
            return await _storeContext.ClinicRatings
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
        }
        
        public async Task AddRatingAsync(ClinicRating rating)
        {
            await _storeContext.ClinicRatings.AddAsync(rating);
        }
        
        public void UpdateRating(ClinicRating rating)
        {
            _storeContext.ClinicRatings.Update(rating);
        }
        
        public void DeleteRating(ClinicRating rating)
        {
            _storeContext.ClinicRatings.Remove(rating);
        }
        
        public async Task<double> GetAverageRatingAsync(Guid clinicId)
        {
            var ratings = await _storeContext.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .Select(r => r.Rating)
                .ToListAsync();
                
            return ratings.Any() ? ratings.Average() : 0;
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








