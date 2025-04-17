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
    public class PrescriptionRepository : GenericRepository<Prescription, Guid>, IPrescriptionRepository
    {
        private readonly StoreContext _context;
        
        public PrescriptionRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Prescription> GetPrescriptionByNumberAsync(string prescriptionNumber)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .FirstOrDefaultAsync(p => p.PrescriptionNumber == prescriptionNumber);
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPetIdAsync(Guid petProfileId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.PetProfileId == petProfileId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync()
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.EndDate >= currentDate)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByStatusAsync(PrescriptionStatus status)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.Status == status.ToString())
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetPagedPrescriptionsAsync(
            Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<Prescription> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }
        
        // Implement IRepositoryBase<Prescription, Guid> interface methods
        public async Task<IEnumerable<Prescription>> ListAsync(Core.Common.Specifications.ISpecification<Prescription> spec)
        {
            return await SpecificationEvaluator<Prescription>.GetQuery(_context.Prescriptions.AsQueryable(), spec).ToListAsync();
        }
        
        public async Task<Prescription> FirstOrDefaultAsync(Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<Prescription> spec)
        {
            var specification = new SpecificationAdapter<Prescription>(spec);
            return await GetAsync(specification);
        }
        
        public async Task<int> CountAsync(Core.Common.Specifications.ISpecification<Prescription> spec)
        {
            return await SpecificationEvaluator<Prescription>.GetQuery(_context.Prescriptions.AsQueryable(), spec).CountAsync();
        }
        
        // Override GetAllAsync for interface compatibility
        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








