global using AutoMapper;
using Core.Services.MappingProfiles;
using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Services;
using Services.Abstractions;
//using Services.Services;
using Shared.SecurityModels;

namespace BitaryProject.Api.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
            
            // Register ServiceManager with IServiceProvider
            services.AddScoped<IServiceManager>(provider => new ServiceManager(
                provider.GetRequiredService<IUnitOFWork>(),
                provider.GetRequiredService<IMapper>(),
                provider.GetRequiredService<IbasketRepository>(),
                provider.GetRequiredService<UserManager<User>>(),
                provider.GetRequiredService<IOptions<JwtOptions>>(),
                provider.GetRequiredService<IOptions<DomainSettings>>(),
                provider.GetRequiredService<IConfiguration>(),
                provider.GetRequiredService<IMailingService>(),
                provider
            ));

            // Services are now managed by ServiceManager
            
            //services.AddScoped<IDoctorService, DoctorService>();
            //services.AddScoped<IClinicService, ClinicService>();
            //services.AddScoped<IAppointmentService, AppointmentService>();
            //services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            //services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();


            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddAutoMapper(typeof(AddressProfile));
            return services;
        }
    }
}
