using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ProductNotFoundException : EntityNotFoundException
    {
        public ProductNotFoundException(string id)
            : base($"The product with ID {id} was not found.")
        {
        }
    }
}
