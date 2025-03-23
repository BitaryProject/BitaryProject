using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IBasketService> _lazyBasketService;
        
        private readonly Lazy<IOrderService> _lazyOrderService;

        private readonly Lazy<IAuthenticationService> _authenticationService;

        private readonly Lazy<IPaymentService> _paymentService;

        public ServiceManager(IUnitOFWork unitOFWork, IMapper mapper ,IbasketRepository basketRepository,UserManager<User> userManager,IOptions<JwtOptions> options
            ,IOptions<DomainSettings> options1,IConfiguration configuration,IMailingService mailingService)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOFWork, mapper));
            _lazyBasketService = new Lazy<IBasketService>(() => new BasketServic(basketRepository, mapper));
            _lazyOrderService = new Lazy<IOrderService>(() => new OrderService(unitOFWork, mapper, basketRepository));

            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, options, options1, mapper,mailingService));
            _paymentService = new Lazy<IPaymentService>(() => new PaymentService(basketRepository, unitOFWork, mapper, configuration));

        } 
        public IProductService ProductService => _productService.Value;

        public IBasketService BasketService => _lazyBasketService.Value;
        
        public IOrderService OrderService =>_lazyOrderService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public IPaymentService PaymentService => _paymentService.Value;
    }
}
  