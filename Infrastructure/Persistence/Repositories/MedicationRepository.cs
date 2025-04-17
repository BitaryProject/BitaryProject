using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class MedicationRepository : IRepositoryBase<Medication, Guid>, IMedicationRepository
    {
        private readonly StoreContext _dbContext;

        public MedicationRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Medication entity)
        {
            await _dbContext.Medications.AddAsync(entity);
        }

        public async Task<int> CountAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public void Delete(Medication entity)
        {
            _dbContext.Medications.Remove(entity);
        }

        public async Task<Medication> FindByNameAsync(string name)
        {
            return await _dbContext.Medications
                .FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
        }

        public async Task<Medication> FirstOrDefaultAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await _dbContext.Medications.ToListAsync();
        }

        public async Task<Medication> GetAsync(Guid id)
        {
            return await _dbContext.Medications.FindAsync(id);
        }

        public async Task<IEnumerable<Medication>> GetMedicationsByCategoryAsync(string category)
        {
            return await _dbContext.Medications
                .Where(m => m.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> ListAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
        {
            return await _dbContext.Medications
                .Where(m => m.Name.Contains(searchTerm) || 
                       m.Description.Contains(searchTerm) ||
                       m.Manufacturer.Contains(searchTerm))
                .ToListAsync();
        }

        public void Update(Medication entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        private IQueryable<Medication> ApplySpecification(ISpecification<Medication> spec)
        {
            return SpecificationEvaluator<Medication>.GetQuery(_dbContext.Medications.AsQueryable(), spec);
        }
    }
} 