//using Domain.Entities;
//using Domain.Contracts;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Persistence.Repositories.NewModule
//{
//    public class NewModuleGenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
//        where TEntity : BaseEntity<TKey>
//    {
//        protected readonly NewModuleContext _context;
//        private readonly DbSet<TEntity> _dbSet;

//        public NewModuleGenericRepository(NewModuleContext context)
//        {
//            _context = context;
//            _dbSet = _context.Set<TEntity>();
//        }

//        public async Task AddAsync(TEntity entity)
//            => await _dbSet.AddAsync(entity);

//        public void Delete(TEntity entity)
//            => _dbSet.Remove(entity);

//        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
//        {
//            if (trackChanges)
//                return await _dbSet.ToListAsync();
//            return await _dbSet.AsNoTracking().ToListAsync();
//        }

//        public async Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications)
//            => await ApplySpecifications(specifications).ToListAsync();

//        public async Task<TEntity?> GetAsync(TKey id)
//            => await _dbSet.FindAsync(id);

//        public async Task<TEntity?> GetAsync(Specifications<TEntity> specifications)
//            => await ApplySpecifications(specifications).FirstOrDefaultAsync();

//        public void Update(TEntity entity)
//            => _dbSet.Update(entity);

//        private IQueryable<TEntity> ApplySpecifications(Specifications<TEntity> specifications)
//            => SpecificationEvaluator.GetQuery<TEntity>(_dbSet, specifications);

//        public async Task<int> CountAsync(Specifications<TEntity> specifications)
//            => await ApplySpecifications(specifications).CountAsync();

//        public IQueryable<TEntity> GetAllAsQueryable()
//            => _dbSet.AsQueryable();

//        public async Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetPagedAsync(Specifications<TEntity> specifications, int pageIndex, int pageSize)
//        {
//            var query = SpecificationEvaluator.GetQuery<TEntity>(_dbSet, specifications);
//            int totalCount = await query.CountAsync();
//            var pagedEntities = await query.Skip((pageIndex - 1) * pageSize)
//                                           .Take(pageSize)
//                                           .ToListAsync();
//            return (pagedEntities, totalCount);
//        }
//    }
//}
