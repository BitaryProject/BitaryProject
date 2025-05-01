using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<TEntity?> GetAsync(TKey id);
        Task<TEntity?> GetAsync(Specifications<TEntity> specifications);
        Task<int> CountAsync(Specifications<TEntity> specifications);

        Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false);

        Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications);

        Task AddAsync(TEntity entity);

        void Delete(TEntity entity);
        void Update(TEntity entity);
        IQueryable<TEntity> GetAllAsQueryable();

        Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetPagedAsync(Specifications<TEntity> specifications, int pageIndex, int pageSize);
        
        Task<IEnumerable<TKey>> GetAllIdsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
