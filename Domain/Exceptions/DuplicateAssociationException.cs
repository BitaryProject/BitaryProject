using System;

namespace Domain.Exceptions
{
    public class DuplicateAssociationException : Exception
    {
        public DuplicateAssociationException() : base("This association already exists.")
        {
        }

        public DuplicateAssociationException(string message) : base(message)
        {
        }

        public DuplicateAssociationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 