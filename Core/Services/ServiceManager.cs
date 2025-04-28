using Domain.Contracts;
//using Domain.Contracts.NewModule;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Abstractions;
//using Services.Services;
using Shared.SecurityModels;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AutoMapper;
using Domain.Contracts;
using Persistence.Data;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IBasketService> _basketService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IPaymentService> _paymentService;
        private readonly Lazy<IPetService> _petService;
        private readonly Lazy<IDoctorService> _doctorService;
        private readonly Lazy<IClinicService> _clinicService;
        private readonly Lazy<IDoctorScheduleService> _doctorScheduleService;
        private readonly Lazy<IAppointmentService> _appointmentService;
        private readonly AutoMapper.IMapper _mapper;
        private readonly Lazy<IMedicalRecordService> _medicalRecordService;
        private readonly Lazy<IRatingService> _ratingService;
        private readonly Lazy<IWishListService> _wishListService;

        //private readonly Lazy<IClinicSearchService> _clinicSearchService;

        public ServiceManager(
            IUnitOFWork unitOfWork,
            AutoMapper.IMapper mapper,
            IbasketRepository basketRepository,
            UserManager<User> userManager,
            IOptions<JwtOptions> jwtOptions,
            IOptions<DomainSettings> domainSettings,
            IConfiguration configuration,
            IMailingService mailingService,
            IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));
            _basketService = new Lazy<IBasketService>(() => new BasketService(basketRepository, mapper, unitOfWork));
            _orderService = new Lazy<IOrderService>(() => new OrderService(unitOfWork, mapper, basketRepository));
            _doctorService = new Lazy<IDoctorService>(() => new DoctorService(unitOfWork, mapper, userManager));
            
            // Initialize authenticationService after doctorService
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                userManager, 
                jwtOptions, 
                domainSettings, 
                mapper, 
                mailingService,
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                _doctorService.Value
            ));
            _paymentService = new Lazy<IPaymentService>(() => new PaymentService(basketRepository, unitOfWork, mapper, configuration));
            _petService = new Lazy<IPetService>(() => new PetService(
                serviceProvider.GetRequiredService<IPetRepository>(), 
                mapper,
                LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PetService>()));
                
            _clinicService = new Lazy<IClinicService>(() => new ClinicService(unitOfWork, mapper, userManager));
            _doctorScheduleService = new Lazy<IDoctorScheduleService>(() => new DoctorScheduleService(unitOfWork, mapper));
            
            // Use the AppointmentService from Core/Services
            _appointmentService = new Lazy<IAppointmentService>(() => new AppointmentService(unitOfWork, mapper));
            
            // Use the MedicalRecordService from Core/Services
            _medicalRecordService = new Lazy<IMedicalRecordService>(() => new MedicalRecordService(
                unitOfWork, 
                mapper, 
                _appointmentService.Value));
                
            // Initialize the RatingService
            _ratingService = new Lazy<IRatingService>(() => new RatingService(
                unitOfWork,
                mapper,
                _clinicService.Value));
                
            // Initialize the WishListService
            _wishListService = new Lazy<IWishListService>(() => new WishListService(
                unitOfWork,
                mapper,
                _productService.Value));

            //_petService = new Lazy<IPetService>(() => new PetService(petRepository, mapper));
            //_doctorService = new Lazy<IDoctorService>(() => new DoctorService(doctorRepository, mapper));
            //_appointmentService = new Lazy<IAppointmentService>(() => new AppointmentService(appointmentRepository, mapper));
            //_clinicService = new Lazy<IClinicService>(() => new ClinicService(clinicRepository, mapper));
            //_medicalRecordService = new Lazy<IMedicalRecordService>(() => new MedicalRecordService(medicalRecordRepository, mapper));

            //_clinicSearchService = new Lazy<IClinicSearchService>(() => clinicSearchService);
            //_doctorScheduleService = new Lazy<IDoctorScheduleService>(() => doctorScheduleService);
        }

        public IProductService ProductService => _productService.Value;
        public IBasketService BasketService => _basketService.Value;
        public IOrderService OrderService => _orderService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IPaymentService PaymentService => _paymentService.Value;
        public IPetService PetService => _petService.Value;
        public IDoctorService DoctorService => _doctorService.Value;
        public IClinicService ClinicService => _clinicService.Value;
        public IDoctorScheduleService DoctorScheduleService => _doctorScheduleService.Value;
        public IAppointmentService AppointmentService => _appointmentService.Value;
        public IMedicalRecordService MedicalRecordService => _medicalRecordService.Value;
        public IRatingService RatingService => _ratingService.Value;
        public IWishListService WishListService => _wishListService.Value;
        //public IClinicSearchService ClinicSearchService => _clinicSearchService.Value;
    }
}