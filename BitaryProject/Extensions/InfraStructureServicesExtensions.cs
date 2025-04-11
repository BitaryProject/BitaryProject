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
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Domain.Contracts.NewModule;
using Persistence.Repositories.NewModule;
using Services.Abstractions;
using Services;
namespace BitaryProject.Extensions
{
    public static class InfraStructureServicesExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOFWork, UnitOfWork>();
            services.AddScoped<IbasketRepository, BasketRepository>();
            services.AddScoped<INewModuleUnitOfWork, NewModuleUnitOfWork>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
            services.AddScoped<IClinicSearchService, ClinicSearchService>();

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

              services.AddDbContext<NewModuleContext>(
               options =>
               { 
                options.UseSqlServer(configuration.GetConnectionString("NewModuleConnection"));
               }
               );

            services.Configure<DomainSettings>(configuration.GetSection("DomainUrls"));

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
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailTokenProvider";


            }).AddEntityFrameworkStores<IdentityContext>()
                                .AddDefaultTokenProviders()
                        .AddTokenProvider<CustomEmailTokenProvider<User>>("CustomEmailTokenProvider"); 






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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    NameClaimType = ClaimTypes.Email

                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
