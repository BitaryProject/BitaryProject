using Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public class UpdatePaymentStatusRequest
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public OrderPaymentStatus Status { get; set; }
    }
} 