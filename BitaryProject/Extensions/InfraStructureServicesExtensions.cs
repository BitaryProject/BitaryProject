global using Domain.Contracts;
global using Microsoft.EntityFrameworkCore;
global using Persistence;
global using Persistence.Data;
using Persistence.Repositories;
using StackExchange.Redis;

namespace BitaryProject.Extensions
{
    public static class InfraStructureServicesExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOFWork, UnitOfWork>();
            services.AddScoped<IbasketRepository, BasketRepository>();
            services.AddDbContext<StoreContext>(
               options =>
               {
                   options.UseSqlServer(configuration.GetConnectionString("DefaultSQLConnection"));
               }
               );


            services.AddSingleton<IConnectionMultiplexer>
                  (_ => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            return services;

        }
    }
}
