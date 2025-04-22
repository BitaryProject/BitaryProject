/*
using Domain.Entities.ClinicEntities;
using Domain.Contracts.NewModule;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.NewModule
{
    public class ClinicRepository : NewModuleGenericRepository<Clinic, int>, IClinicRepository
    {
        public ClinicRepository(NewModuleContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Clinic>> GetClinicsByCityAsync(string city)
        {
            return await _context.Clinics
                .Where(c => c.Address.City.ToLowerInvariant() == city.ToLowerInvariant())
                .ToListAsync();
        }

        public async Task<IEnumerable<Clinic>> GetClinicsByNameAsync(string clinicName)
        {
            return await _context.Clinics
                         .Where(c => c.ClinicName.ToLowerInvariant().Contains(clinicName.ToLowerInvariant()))
                         .ToListAsync();
        }

        public async Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count)
        {
            return await _context.Clinics
                .OrderByDescending(c => c.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Clinic>> GetClinicsByStatusAsync(ClinicStatus status)
        {
            return await _context.Clinics
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Clinic>> GetClinicsByOwnerIdAsync(string ownerId)
        {
            return await _context.Clinics
                .Where(c => c.OwnerId == ownerId)
                .ToListAsync();
        }

        public override async Task<Clinic> GetAsync(int id)
        {
            return await _context.Clinics
                .Include(c => c.Doctors)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<Clinic>> GetAllAsync()
        {
            return await _context.Clinics
                .Include(c => c.Doctors)
                .ToListAsync();
        }
    }
}
*/
