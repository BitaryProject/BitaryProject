using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException(Guid id) 
            : base($"No order with Id {id} was found.")
        {
        }
        
        public OrderNotFoundException(Guid id, string message)
            : base(message)
        {
        }
    }
}
