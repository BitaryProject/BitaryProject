﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public record OrderResult
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public AddressDTO ShippingAddress { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public string PaymentStatus { get; set; } 
        public string DeliveryMethod { get; set; }
        public decimal Subtotal { get; set; }
        public string PaymentIntentId { get; set; }

        public decimal Total { get; set; }   
    }
}
