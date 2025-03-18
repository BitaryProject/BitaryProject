global using Domain.Entities;
global using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreContext _storeContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
            _dbSet = _storeContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity) => await _storeContext.Set<TEntity>().AddAsync(entity);


        public void Delete(TEntity entity) => _storeContext.Set<TEntity>().Remove(entity);


        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
        {
            // lw heya no tracking e3ml leha retrieve lw tracking e3mlha no tracking ba3dha retrieve 
            if (trackChanges)
                return await _storeContext.Set<TEntity>().ToListAsync();

            return await _storeContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications)

            => await ApplySpecifications(specifications).ToListAsync();

        public async Task<TEntity?> GetAsync(TKey id) => await _storeContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetAsync(Specifications<TEntity> specifications)

         => await ApplySpecifications(specifications).FirstOrDefaultAsync();


        public void Update(TEntity entity) => _storeContext.Set<TEntity>().Update(entity);

        private IQueryable<TEntity> ApplySpecifications(Specifications<TEntity> specifications)
            => SpecificationEvaluator.GetQuery<TEntity>(_storeContext.Set<TEntity>(), specifications);
        public async Task<int> CountAsync(Specifications<TEntity> specifications)
                   => await ApplySpecifications(specifications).CountAsync();

        public IQueryable<TEntity> GetAllAsQueryable() => _dbSet.AsQueryable();

    }
}
