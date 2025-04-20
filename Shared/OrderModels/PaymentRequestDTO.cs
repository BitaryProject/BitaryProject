using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public class PaymentRequestDTO
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
} 