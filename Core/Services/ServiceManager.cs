using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
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
        public ServiceManager(IUnitOFWork unitOFWork, IMapper mapper ,IbasketRepository basketRepository,UserManager<User> userManager,IOptions<JwtOptions> options)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOFWork, mapper));
            _lazyBasketService = new Lazy<IBasketService>(() => new BasketServic(basketRepository, mapper));
            _lazyOrderService = new Lazy<IOrderService>(() => new OrderService(unitOFWork, mapper, basketRepository));

            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, options));

          } 
        public IProductService ProductService => _productService.Value;

        public IBasketService BasketService => _lazyBasketService.Value;
        
        public IOrderService OrderService =>_lazyOrderService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

    }
}
  