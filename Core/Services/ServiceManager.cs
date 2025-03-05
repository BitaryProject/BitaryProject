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
        public ServiceManager(IUnitOFWork unitOFWork, IMapper mapper)
        {
            _productService = new Lazy<IProductService>(() => new ProductService(unitOFWork, mapper));
           
        }

        public IProductService ProductService => _productService.Value;

    }
}
