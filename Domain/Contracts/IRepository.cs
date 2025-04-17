using Core.Services.Specifications.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(ISpecification<T> spec);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<(IEnumerable<T> Entities, int TotalCount)> GetPagedAsync(ISpecification<T> spec, int pageIndex, int pageSize);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
} 