global using AutoMapper;
using Services;
using Services.Abstractions;

namespace BitaryProject.Api.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
            services.AddScoped<IServiceManager, ServiceManager>();

            return services;
        }
    }
}
