using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public class UpdateDeliveryMethodRequest
    {
        [Required]
        public int DeliveryMethodId { get; set; }
    }
} 