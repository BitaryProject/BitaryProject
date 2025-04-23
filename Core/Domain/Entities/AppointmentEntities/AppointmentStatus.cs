using System;

namespace Domain.Entities.AppointmentEntities
{
    public enum AppointmentStatus : byte
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Completed = 4,
        Cancelled = 5
    }
} 