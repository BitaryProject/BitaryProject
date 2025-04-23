using System;

namespace Domain.Exceptions
{
    public class MedicalRecordNotFoundException : EntityNotFoundException
    {
        public MedicalRecordNotFoundException(string id)
            : base($"The medical record with ID {id} was not found.")
        {
        }
    }
} 