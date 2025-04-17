using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistence.Repositories.HealthcareRepositories;

namespace Persistence.Repositories.HealthcareRepositories
{
    public class PrescriptionRepository : GenericRepository<Prescription, Guid>, IPrescriptionRepository
    {
        private readonly StoreContext _context;
        
        public PrescriptionRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPetProfileIdAsync(Guid petProfileId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.PetProfileId == petProfileId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.DoctorId == doctorId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsForPetAsync(Guid petProfileId)
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.PetProfileId == petProfileId && 
                       p.EndDate >= currentDate)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.MedicalRecordId == medicalRecordId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetPagedPrescriptionsAsync(
            ISpecification<Prescription> specification, int pageIndex, int pageSize)
        {
            var spec = new SpecificationAdapter<Prescription>(specification);
            var (Entities, TotalCount) = await GetPagedAsync(spec, pageIndex, pageSize);
            return (Entities, TotalCount);
        }
        
        // Implement IRepositoryBase<Prescription, Guid> interface methods
        public async Task<IEnumerable<Prescription>> ListAsync(ISpecification<Prescription> spec)
        {
            var specification = new SpecificationAdapter<Prescription>(spec);
            return await GetAllAsync(specification);
        }
        
        public async Task<Prescription> FirstOrDefaultAsync(ISpecification<Prescription> spec)
        {
            var specification = new SpecificationAdapter<Prescription>(spec);
            return await GetAsync(specification);
        }
        
        public async Task<int> CountAsync(ISpecification<Prescription> spec)
        {
            var specification = new SpecificationAdapter<Prescription>(spec);
            return await base.CountAsync(specification);
        }
        
        // Override GetAllAsync for interface compatibility
        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
    }
} 