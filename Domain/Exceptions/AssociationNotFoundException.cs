using System;

namespace Domain.Exceptions
{
    public class AssociationNotFoundException : Exception
    {
        public AssociationNotFoundException() : base("The requested association was not found.")
        {
        }

        public AssociationNotFoundException(string message) : base(message)
        {
        }

        public AssociationNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 