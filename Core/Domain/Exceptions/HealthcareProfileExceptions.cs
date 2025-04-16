using System;
using System.Collections.Generic;

namespace Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a healthcare profile creation fails
    /// </summary>
    public class HealthcareProfileCreationException : Exception
    {
        public string ProfileType { get; }
        public string UserId { get; }

        public HealthcareProfileCreationException(string profileType, string userId, string message) 
            : base(message)
        {
            ProfileType = profileType;
            UserId = userId;
        }

        public HealthcareProfileCreationException(string profileType, string userId, string message, Exception innerException) 
            : base(message, innerException)
        {
            ProfileType = profileType;
            UserId = userId;
        }
    }

    /// <summary>
    /// Exception thrown when a healthcare profile is not found
    /// </summary>
    public class HealthcareProfileNotFoundException : Exception
    {
        public string ProfileType { get; }
        public string UserId { get; }

        public HealthcareProfileNotFoundException(string profileType, string userId)
            : base($"{profileType} profile not found for user ID {userId}")
        {
            ProfileType = profileType;
            UserId = userId;
        }
    }

    /// <summary>
    /// Exception thrown when a healthcare profile update fails
    /// </summary>
    public class HealthcareProfileUpdateException : Exception
    {
        public string ProfileType { get; }
        public string ProfileId { get; }

        public HealthcareProfileUpdateException(string profileType, string profileId, string message)
            : base(message)
        {
            ProfileType = profileType;
            ProfileId = profileId;
        }

        public HealthcareProfileUpdateException(string profileType, string profileId, string message, Exception innerException)
            : base(message, innerException)
        {
            ProfileType = profileType;
            ProfileId = profileId;
        }
    }
} 