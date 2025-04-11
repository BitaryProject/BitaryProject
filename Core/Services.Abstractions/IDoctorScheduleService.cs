using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IDoctorScheduleService
    {
        Task AddScheduleAsync(Guid doctorId, DoctorScheduleDTO schedule);
        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime date);
    }
}
