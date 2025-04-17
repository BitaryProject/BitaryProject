using Core.Services;
using Core.Services.Abstractions;
using Core.Domain.Contracts;
using Core.Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SecurityModels;
using System;
using AutoMapper;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IBasketService> _basketService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IPaymentService> _paymentService;
        private readonly Lazy<IPrescriptionService> _prescriptionService;
        private readonly Lazy<IMedicalRecordService> _medicalRecordService;
        private readonly Lazy<IMedicalNoteService> _medicalNoteService;
        private readonly Lazy<IDoctorService> _doctorService;
        private readonly Lazy<IClinicService> _clinicService;
        private readonly Lazy<IPetProfileService> _petProfileService;
        private readonly Lazy<IPetOwnerService> _petOwnerService;
        private readonly Lazy<IAppointmentService> _appointmentService;
        private readonly Lazy<IRatingService> _ratingService;

        public ServiceManager(
            IUnitOFWork unitOfWork,
            IMapper mapper,
            IbasketRepository basketRepository,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtOptions> jwtOptions,
            IOptions<DomainSettings> domainSettings,
            IConfiguration configuration,
            IMailingService mailingService,
            IHealthcareUnitOfWork healthcareUnitOfWork,
            ILoggerFactory loggerFactory)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));
            _basketService = new Lazy<IBasketService>(() => new BasketService(basketRepository, mapper));
            _orderService = new Lazy<IOrderService>(() => new OrderService(unitOfWork, mapper, basketRepository));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                userManager,
                roleManager,
                jwtOptions,
                domainSettings,
                mapper,
                mailingService,
                healthcareUnitOfWork,
                loggerFactory.CreateLogger<AuthenticationService>()
            ));
            _paymentService = new Lazy<IPaymentService>(() => new PaymentService(basketRepository, unitOfWork, mapper, configuration));
            _prescriptionService = new Lazy<IPrescriptionService>(() => new PrescriptionService(
                healthcareUnitOfWork,
                mapper
            ));
            _medicalRecordService = new Lazy<IMedicalRecordService>(() => new MedicalRecordService(healthcareUnitOfWork, mapper));
            _medicalNoteService = new Lazy<IMedicalNoteService>(() => new MedicalNoteService(
                healthcareUnitOfWork, 
                mapper, 
                loggerFactory.CreateLogger<MedicalNoteService>()
            ));
            _doctorService = new Lazy<IDoctorService>(() => new DoctorService(healthcareUnitOfWork, mapper));
            _clinicService = new Lazy<IClinicService>(() => new ClinicService(healthcareUnitOfWork, mapper));
            _petProfileService = new Lazy<IPetProfileService>(() => new PetProfileService(healthcareUnitOfWork, mapper, loggerFactory.CreateLogger<PetProfileService>()));
            _petOwnerService = new Lazy<IPetOwnerService>(() => new PetOwnerService(healthcareUnitOfWork, mapper, loggerFactory.CreateLogger<PetOwnerService>()));
            _appointmentService = new Lazy<IAppointmentService>(() => new AppointmentService(healthcareUnitOfWork, mapper, loggerFactory.CreateLogger<AppointmentService>()));
            _ratingService = new Lazy<IRatingService>(() => new RatingService(healthcareUnitOfWork, mapper, loggerFactory.CreateLogger<RatingService>()));
        }

        public IProductService ProductService => _productService.Value;
        public IBasketService BasketService => _basketService.Value;
        public IOrderService OrderService => _orderService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IPaymentService PaymentService => _paymentService.Value;
        public IPrescriptionService PrescriptionService => _prescriptionService.Value;
        public IMedicalRecordService MedicalRecordService => _medicalRecordService.Value;
        public IMedicalNoteService MedicalNoteService => _medicalNoteService.Value;
        public IDoctorService DoctorService => _doctorService.Value;
        public IClinicService ClinicService => _clinicService.Value;
        public IPetProfileService PetProfileService => _petProfileService.Value;
        public IPetOwnerService PetOwnerService => _petOwnerService.Value;
        public IAppointmentService AppointmentService => _appointmentService.Value;
        public IRatingService RatingService => _ratingService.Value;
    }
}
