using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.OrderEntities
{
    public class Order: BaseEntity<Guid>
    {
        //private Address shippingAddress;
        //private List<OrderItem> orderItems;
        //private DeliveryMethod deliveryMethod;

        public Order()
        {
            
        }

        //public Order(string userEmail, Address shippingAddress, List<OrderItem> orderItems, DeliveryMethod deliveryMethod, decimal subtotal)
        //{
        //    UserEmail = userEmail;
        //    this.shippingAddress = shippingAddress;
        //    this.orderItems = orderItems;
        //    this.deliveryMethod = deliveryMethod;
        //    Subtotal = subtotal;
        //}

        public Order(string userEmail,
            Address shippingAddress,
            ICollection<OrderItem> orderItems,
            DeliveryMethod deliveryMethod,
            decimal subtotal,
            string paymentIntentId
            )
        {
            Id= Guid.NewGuid();
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            OrderItems = orderItems;
            DeliveryMethod = deliveryMethod;
            Subtotal = subtotal;
            PaymentIntentId = paymentIntentId;
        }

        public string UserEmail { get; set; } 
        public Address ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderPaymentStatus PaymentStatus { get; set; } = OrderPaymentStatus.Pending;
        public DeliveryMethod DeliveryMethod { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal Subtotal { get; set; }
        public string PaymentIntentId { get; set; } 

    }
}
