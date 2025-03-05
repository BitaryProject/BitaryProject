global using Domain.Contracts;
global using Microsoft.EntityFrameworkCore;
global using Persistence;
global using Persistence.Data;
using Persistence.Repositories;

namespace BitaryProject.Extensions
{
    public static class InfraStructureServicesExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOFWork, UnitOfWork>();

            services.AddDbContext<StoreContext>(
               options =>
               {
                   options.UseSqlServer(configuration.GetConnectionString("DefaultSQLConnection"));
               }
               );
            return services;
        }
    }
}
