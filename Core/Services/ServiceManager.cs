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
        public ServiceManager(IUnitOFWork unitOFWork, IMapper mapper ,IbasketRepository basketRepository)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOFWork, mapper));
            _lazyBasketService = new Lazy<IBasketService>(() => new BasketServic(basketRepository, mapper));
            _lazyOrderService = new Lazy<IOrderService>(() => new OrderService(unitOFWork, mapper, basketRepository));
           
        }

        public IProductService ProductService => _productService.Value;

        public IBasketService BasketService => _lazyBasketService.Value;

        public IOrderService OrderService =>_lazyOrderService.Value;
    }
}
  