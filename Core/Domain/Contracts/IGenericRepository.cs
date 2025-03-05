using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
