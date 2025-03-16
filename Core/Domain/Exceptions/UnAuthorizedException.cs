using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class UnAuthorizedException(string message="Invalid Email or Password")
        :Exception(message)
    {
    }
}
