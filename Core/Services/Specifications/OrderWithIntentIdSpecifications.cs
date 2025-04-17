global using Core.Domain.Entities.OrderEntities;
global using Core.Domain.Contracts;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Specifications
{
    public class OrderWithPaymentIntentIdSpecifications : Specifications<Order>
    {
        public OrderWithPaymentIntentIdSpecifications(string paymentIntentId)
            : base(order => order.PaymentIntentId == paymentIntentId)
        {
        }
    }

}
