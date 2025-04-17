using Microsoft.Extensions.DependencyInjection;
using API.Services;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IPetProfileService, PetProfileService>();
            services.AddScoped<IPetOwnerService, PetOwnerService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IRatingService, RatingService>();
            return services;
        }
    }
} 