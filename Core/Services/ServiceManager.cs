using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IBasketService> _lazyBasketService;
        public ServiceManager(IUnitOFWork unitOFWork, IMapper mapper ,IbasketRepository basketRepository)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOFWork, mapper));
            _lazyBasketService = new Lazy<IBasketService>(() => new BasketServic(basketRepository, mapper));
           
        }

        public IProductService ProductService => _productService.Value;

        public IBasketService BasketService => _lazyBasketService.Value;
    }
}
  