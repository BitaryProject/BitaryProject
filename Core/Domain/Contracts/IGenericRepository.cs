using Core.Common.Specifications;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        Task<T> GetByIdAsync(TKey id);
        
        Task<T> GetAsync(Core.Common.Specifications.ISpecification<T> spec);
        
        Task<T> GetAsync(Expression<Func<T>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<IEnumerable<T>> GetAllAsync(Core.Common.Specifications.ISpecification<T> spec);
        
        Task<IEnumerable<T>> GetAllWithSpecAsync(Core.Common.Specifications.ISpecification<T> spec);
        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(Core.Common.Specifications.ISpecification<T> spec, int pageIndex, int pageSize);
        
        Task<int> CountAsync(Core.Common.Specifications.ISpecification<T> spec);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        
        Task<bool> AnyAsync(Core.Common.Specifications.ISpecification<T> spec);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        Task<IEnumerable<T>> ListAsync(Core.Common.Specifications.ISpecification<T> spec);
    }
}


