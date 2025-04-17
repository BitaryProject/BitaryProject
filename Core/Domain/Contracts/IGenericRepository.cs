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
        Task<T> GetAsync(ISpecification<T> spec);
        Task<T> GetAsync(Expression<Func<T>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(ISpecification<T> spec, int pageIndex, int pageSize);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(ISpecification<T> spec);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> ListAsync(ISpecification<T> spec);
    }
}


