global using AutoMapper;
using Core.Services.MappingProfiles;
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
            services.AddScoped<IServiceManager, ServiceManager>();

          /*  services.AddScoped<IPetService, PetService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
          */

            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddAutoMapper(typeof(AddressProfile));
            return services;
        }
    }
}
