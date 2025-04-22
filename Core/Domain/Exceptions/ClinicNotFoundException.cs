using System;

namespace Domain.Exceptions
{
    public class ClinicNotFoundException : EntityNotFoundException
    {
        public ClinicNotFoundException(string id)
            : base($"The clinic with ID {id} was not found.")
        {
        }
    }
    
    public class DoctorNotFoundException : EntityNotFoundException
    {
        public DoctorNotFoundException(string id)
            : base($"The doctor with ID {id} was not found.")
        {
        }
    }
} 