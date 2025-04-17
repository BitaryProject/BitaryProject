using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();
        
        // Overload for GetRepository with two type parameters
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
        
        // Overload for GetRepository with one type parameter (defaults to Guid)
        IGenericRepository<TEntity, Guid> GetRepository<TEntity>() where TEntity : BaseEntity<Guid>;
    }
    
    // Keep the old interface for backward compatibility
    public interface IUnitOFWork
    {
        public Task<int> SaveChangesAsync();
        
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    }
} 