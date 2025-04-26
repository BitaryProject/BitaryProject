using System;

namespace Domain.Exceptions
{
    public class RatingNotFoundException : EntityNotFoundException
    {
        public RatingNotFoundException(string id)
            : base($"The rating with ID {id} was not found.")
        {
        }
    }
} 