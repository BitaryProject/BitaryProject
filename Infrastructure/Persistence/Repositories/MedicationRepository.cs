using System.Linq.Expressions;
using Core.Common.Specifications;
using Core.Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class MedicationRepository : IRepositoryBase<Medication, Guid>, IMedicationRepository
    {
        private readonly StoreContext _dbContext;

        public MedicationRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Medication> AddAsync(Medication entity)
        {
            await _dbContext.Medications.AddAsync(entity);
            return entity;
        }

        public async Task<int> CountAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbContext.Medications.CountAsync(predicate);
        }

        public void Delete(Medication entity)
        {
            _dbContext.Medications.Remove(entity);
        }

        public async Task<IEnumerable<Medication>> FindAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbContext.Medications.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Medication>> FindAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
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

        public async Task<Medication> FirstOrDefaultAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbContext.Medications.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await _dbContext.Medications.ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetAllAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetAllWithSpecAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<Medication> GetAsync(Guid id)
        {
            return await _dbContext.Medications.FindAsync(id);
        }

        public async Task<Medication> GetAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<Medication> GetAsync(Expression<Func<Medication>> predicate)
        {
            return await _dbContext.Medications.FirstOrDefaultAsync(m => predicate.Compile().Invoke() == null);
        }

        public async Task<Medication> GetByIdAsync(Guid id)
        {
            return await _dbContext.Medications.FindAsync(id);
        }

        public async Task<IEnumerable<Medication>> GetMedicationsByCategoryAsync(string category)
        {
            return await _dbContext.Medications
                .Where(m => m.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetMedicationsBySearchTermAsync(string searchTerm)
        {
            return await SearchMedicationsAsync(searchTerm);
        }

        public async Task<(IEnumerable<Medication> Items, int TotalCount)> GetPagedAsync(ISpecification<Medication> spec, int pageIndex, int pageSize)
        {
            var query = ApplySpecification(spec);
            var total = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<bool> AnyAsync(ISpecification<Medication> spec)
        {
            return await ApplySpecification(spec).AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbContext.Medications.AnyAsync(predicate);
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










