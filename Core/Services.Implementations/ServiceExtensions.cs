using Core.Domain.Contracts;
using Core.Services.Abstractions;
using Core.Services.Implementations.Infrastructure;
using Core.Services.Implementations.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Services.Implementations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register mapping profiles
            services.AddAutoMapper(typeof(MedicalNoteProfile).Assembly);
            
            // Register services
            services.AddScoped<IMedicalNoteService, MedicalNoteService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IPetProfileService, PetProfileService>();
            services.AddScoped<IPetOwnerService, PetOwnerService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IRatingService, RatingService>();
            
            // Register all required repositories and UnitOfWork
            // Note: In a production environment, you would use your actual implementation
            // and not the mock ones.
            services.AddScoped<IHealthcareUnitOfWork, MockHealthcareUnitOfWork>();
            
            return services;
        }
    }
} 