using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(string id)
            : base($"The user with ID {id} was not found.")
        {
        }
    }
}
