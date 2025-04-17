using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetAsync(ISpecification<T> spec);
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<(IReadOnlyList<T> Entities, int TotalCount)> GetPagedAsync(ISpecification<T> spec, int pageIndex, int pageSize);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
} 
