using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Specifications;

namespace Core.Domain.Contracts
{
    public interface IRepositoryBase<T, TKey> where T : class
    {
        Task<T> GetAsync(TKey id);
        Task<T> GetAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> FindAsync(ISpecification<T> spec);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
} 
