﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class UserNotFoundException(string email):
        NotFoundException($"No user with Email {email} was found !! .")
    {
    }
}
