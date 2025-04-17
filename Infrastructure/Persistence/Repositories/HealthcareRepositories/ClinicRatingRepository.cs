using Core.Common.Specifications;
using Core.Domain.Contracts;
using Core.Domain.Contracts.HealthcareContracts;
using Core.Domain.Entities.HealthcareEntities;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class ClinicRatingRepository : GenericRepository<ClinicRating, Guid>, IClinicRatingRepository
    {
        private readonly DbContext _context;

        public ClinicRatingRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        // Implementation of IClinicRatingRepository specific methods
        public async Task<IEnumerable<ClinicRating>> GetRatingsByClinicIdAsync(Guid clinicId)
        {
            return await _context.Set<ClinicRating>()
                .Where(r => r.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid clinicId)
        {
            var ratings = await GetRatingsByClinicIdAsync(clinicId);
            return ratings.Any() ? ratings.Average(r => r.Rating) : 0;
        }

        // Other implementation methods as needed
    }
} 











