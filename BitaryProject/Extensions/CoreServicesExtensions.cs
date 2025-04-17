using AutoMapper;
using Core.Services.MappingProfiles;
using Core.Services;
using Core.Services.Abstractions;
using Shared.SecurityModels;
using Core.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BitaryProject.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure AutoMapper
            services.AddAutoMapper(typeof(AddressProfile).Assembly);
            
            // Add Service Manager
            services.AddScoped<IServiceManager, ServiceManager>();

            // Healthcare Services
            services.AddScoped<IPetProfileService, PetProfileService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IPetOwnerService, PetOwnerService>();
            services.AddScoped<IRatingService, RatingService>();

            // E-commerce Services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IPaymentService, PaymentService>();
            
            // Authentication and Communication Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMailingService, MailingService>();

            // Configure JWT
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

            return services;
        }
    }
}
