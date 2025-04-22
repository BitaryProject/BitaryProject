using System;

namespace Domain.Exceptions
{
    public abstract class EntityNotFoundException : Exception
    {
        protected EntityNotFoundException(string message) : base(message)
        {
        }
    }
} 