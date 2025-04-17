using Core.Domain.Entities;
using Core.Common.Specifications;
using Core.Domain.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        protected readonly StoreContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(StoreContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }
        
        public async Task<T> GetAsync(Expression<Func<T>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(x => predicate.Compile().Invoke() == null);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        
        public async Task<IEnumerable<T>> GetAllWithSpecAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(Core.Common.Specifications.ISpecification<T> specification, int pageIndex, int pageSize)
        {
            var query = ApplySpecification(specification);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<int> CountAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }
        
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
        
        public async Task<bool> AnyAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).AnyAsync();
        }
        
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        
        public async Task<IEnumerable<T>> ListAsync(Core.Common.Specifications.ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        protected IQueryable<T> ApplySpecification(Core.Common.Specifications.ISpecification<T> specification)
        {
            if (specification == null)
                return _dbSet.AsQueryable();

            return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
        }
    }
}
