using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOFWork
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


        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        
          => (IGenericRepository<TEntity, TKey>)
                _repositories.GetOrAdd(typeof(TEntity).Name, _ => new GenericRepository<TEntity, TKey>(_storeContext));


            //var typeName =typeof(TEntity).Name;

            //if(_repositories.ContainsKey(typeName)) // lw el dictionay feh esm el type de
            //    return (IGenericRepository<TEntity, TKey>)_repositories[typeName];


            //var repo = new GenericRepository<TEntity, TKey>(_storeContext); // lw fady ba3ml new dictionary

            //_repositories.Add(typeName, repo);
            //return repo;

        


    }
}
