using Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;
namespace Services.Specifications
{
    internal class OrderWithIncludeSpecifications : Specifications<Order>
    {
        public OrderWithIncludeSpecifications(Guid id)
            : base( order => order.Id == id)
        {
            AddInclude(order => order.DeliveryMethod);
            AddInclude( order => order.OrderItems);
        }

        public OrderWithIncludeSpecifications(string email)
            : base( order => order.UserEmail == email)
        {
            AddInclude( order => order.DeliveryMethod);
            AddInclude( order => order.OrderItems);

            setOrderBy( o => o.OrderDate);
        }
    }

}
