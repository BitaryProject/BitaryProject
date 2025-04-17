using Core.Common.Specifications;

using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts;

using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class DoctorRatingRepository : GenericRepository<DoctorRating, Guid>, IDoctorRatingRepository
    {
        private readonly StoreContext _context;
        
        public DoctorRatingRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<DoctorRating>> GetRatingsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
        }
        
        public async Task<double> GetAverageRatingAsync(Guid doctorId)
        {
            var ratings = await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .Select(r => r.Rating)
                .ToListAsync();
                
            return ratings.Any() ? ratings.Average() : 0;
        }
        
        public async Task<DoctorRating> GetRatingByUserAsync(Guid doctorId, Guid userId)
        {
            return await _context.DoctorRatings
                .FirstOrDefaultAsync(r => r.DoctorId == doctorId && r.UserId == userId);
        }
    }
} 








