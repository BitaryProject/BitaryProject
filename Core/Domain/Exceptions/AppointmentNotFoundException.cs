using System;

namespace Domain.Exceptions
{
    public class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException(string id)
            : base($"The appointment with ID {id} was not found.")
        {
        }
    }
} 