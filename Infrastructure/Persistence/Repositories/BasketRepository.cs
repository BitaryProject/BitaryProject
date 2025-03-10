using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities.BasketEntites;
using StackExchange.Redis;

namespace Persistence.Repositories
{
    public class BasketRepository(IConnectionMultiplexer connectionMultiplexer) : IbasketRepository
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
        public async Task<bool> DeleteBasketAsync(string id)
         => await _database.KeyDeleteAsync(id); 

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            var valu = await _database.StringGetAsync(id);
            if (valu.IsNull) return null;
            return JsonSerializer.Deserialize<CustomerBasket>(valu);

        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null)
        {
            var jsonBsket = JsonSerializer.Serialize(basket);
            var isCreatedOrUpdated = await _database
                .StringSetAsync(basket.Id , jsonBsket , timeToLive?? TimeSpan.FromDays(30));
            return isCreatedOrUpdated ? await GetBasketAsync(basket.Id) : null;
        }
    }
}
