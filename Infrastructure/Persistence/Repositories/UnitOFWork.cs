using Core.Common.Specifications;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts;

using Core.Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IUnitOFWork
    {
        private readonly StoreContext _storeContext;
        private readonly ConcurrentDictionary<string, object> _repositories;
        
        public UnitOfWork(StoreContext storeContext)
        {
            _storeContext = storeContext;
            _repositories = new();
        }

        public async Task<int> SaveChangesAsync()
            => await _storeContext.SaveChangesAsync();

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() 
            where TEntity : BaseEntity<TKey>
        {
            return (IGenericRepository<TEntity, TKey>)
                _repositories.GetOrAdd(typeof(TEntity).Name, _ => new GenericRepository<TEntity, TKey>(_storeContext));
        }
        
        // Implement the overloaded version with one type parameter
        public IGenericRepository<TEntity, Guid> GetRepository<TEntity>() 
            where TEntity : BaseEntity<Guid>
        {
            return GetRepository<TEntity, Guid>();
        }
    }
}








