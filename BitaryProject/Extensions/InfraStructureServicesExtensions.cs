global using Domain.Contracts;
global using Microsoft.EntityFrameworkCore;
global using Persistence;
global using Persistence.Data;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;
using Persistence.Repositories;
using Shared.SecurityModels;
using StackExchange.Redis;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

              services.AddDbContext<IdentityContext>(
               options =>
               {
                   options.UseSqlServer(configuration.GetConnectionString("IdentitySQLConnection"));
               }
               );


            services.AddSingleton<IConnectionMultiplexer>
                  (_ => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

            services.ConfigureIdentityService();
            services.ConfigureJwt(configuration);
            return services;

        }

        public static IServiceCollection ConfigureIdentityService(this IServiceCollection services)
        {

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<IdentityContext>();

            return services;
        }

        public static IServiceCollection ConfigureJwt(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            // Ba7ded type el authentication beta3y haykon eh 
            services.AddAuthentication(options =>
            {
                // if authenticated 
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // if not authenticated or fail authentication
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
