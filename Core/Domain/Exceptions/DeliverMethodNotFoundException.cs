using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class DeliverMethodNotFoundException(int id)
       : NotFoundException( $"No Delivery Method with Id {id} was found.");

}
