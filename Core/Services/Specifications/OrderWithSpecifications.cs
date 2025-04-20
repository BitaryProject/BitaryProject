using Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Services.Specifications
{
    internal class OrderWithIncludeSpecifications : Specifications<Order>
    {
        public OrderWithIncludeSpecifications(Guid id)
            : base( order => order.Id == id)
        {
            AddInclude(order => order.DeliveryMethod);
            AddInclude(order => order.OrderItems);
            
            // This is needed to ensure OrderItems data comes through correctly
            AddInclude("OrderItems.Product");
        }

        public OrderWithIncludeSpecifications(string email)
            : base( order => order.UserEmail == email)
        {
            AddInclude(order => order.DeliveryMethod);
            AddInclude(order => order.OrderItems);
            
            // This is needed to ensure OrderItems data comes through correctly
            AddInclude("OrderItems.Product");

            setOrderBy(o => o.OrderDate);
        }
    }
}
