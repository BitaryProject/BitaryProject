global using Core.Domain.Entities.BasketEntities;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Core.Services.Specifications
{
    public class BasketSpecification : Specifications<CustomerBasket>
    {
       
        public BasketSpecification( Guid id)
            : base(b => b.Id == id)
        {
            
            AddInclude(b => b.BasketItems);
        }

        public BasketSpecification(int pageIndex, int pageSize , decimal minShippingPrice)
            : base(b => b.ShippingPrice.HasValue && b.ShippingPrice.Value >= minShippingPrice)
        {
        
            AddInclude(b => b.BasketItems);

            ApplyPagination(pageIndex, pageSize);
            setOrderByDescending(b => b.ShippingPrice);
        }
    }
}

