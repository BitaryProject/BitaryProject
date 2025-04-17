using System;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Extensions
{
    /// <summary>
    /// Extension methods for handling AppointmentStatus enum conversions
    /// between the domain model and service model.
    /// </summary>
    public static class AppointmentStatusExtensions
    {
        /// <summary>
        /// Converts a string status to the domain AppointmentStatus enum
        /// </summary>
        public static AppointmentStatus ToDomainStatus(this string status)
        {
            if (string.IsNullOrEmpty(status))
                return AppointmentStatus.Scheduled;
                
            return Enum.TryParse<AppointmentStatus>(status, true, out var result) 
                ? result 
                : AppointmentStatus.Scheduled;
        }
        
        /// <summary>
        /// Converts a service AppointmentStatus to the domain AppointmentStatus enum
        /// </summary>
        public static AppointmentStatus ToDomainStatus(this AppointmentStatus serviceStatus)
        {
            return (AppointmentStatus)((int)serviceStatus);
        }
        
        /// <summary>
        /// Converts a domain AppointmentStatus to the service AppointmentStatus enum
        /// </summary>
        public static AppointmentStatus ToServiceStatus(this AppointmentStatus domainStatus)
        {
            return (AppointmentStatus)((int)domainStatus);
        }
        
        /// <summary>
        /// Converts a domain AppointmentStatus to string
        /// </summary>
        public static string ToStatusString(this AppointmentStatus status)
        {
            return status.ToString();
        }
        
        /// <summary>
        /// Checks if one status can be transitioned to another
        /// </summary>
        public static bool CanTransitionTo(this AppointmentStatus currentStatus, AppointmentStatus newStatus)
        {
            switch (currentStatus)
            {
                case AppointmentStatus.Scheduled:
                    return newStatus == AppointmentStatus.Confirmed || 
                           newStatus == AppointmentStatus.Cancelled;
                           
                case AppointmentStatus.Confirmed:
                    return newStatus == AppointmentStatus.Completed || 
                           newStatus == AppointmentStatus.Cancelled || 
                           newStatus == AppointmentStatus.NoShow;
                           
                case AppointmentStatus.Completed:
                    return false; // Terminal state
                    
                case AppointmentStatus.Cancelled:
                    return false; // Terminal state
                    
                case AppointmentStatus.NoShow:
                    return false; // Terminal state
                    
                default:
                    return false;
            }
        }
    }
} 