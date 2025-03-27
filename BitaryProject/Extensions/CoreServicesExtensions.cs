global using AutoMapper;
using Core.Services.MappingProfiles;
using Services;
using Services.Abstractions;
using Shared.SecurityModels;

namespace BitaryProject.Api.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
            services.AddScoped<IServiceManager, ServiceManager>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddAutoMapper(typeof(AddressProfile));
            return services;
        }
    }
}
