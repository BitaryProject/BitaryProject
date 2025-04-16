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
    public class ClinicRepository : GenericRepository<Clinic, Guid>, IClinicRepository
    {
        public ClinicRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Clinic>> GetClinicsBySearchTermAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            return await _context.Clinics
                .Include(c => c.Doctors)
                .Where(c => c.Name.ToLower().Contains(searchTerm) || 
                            c.Address.ToLower().Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Specifications<Clinic> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }
    }
} 