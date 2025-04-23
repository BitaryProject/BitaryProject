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
        Task<DoctorScheduleDTO> AddScheduleAsync(int doctorId, DoctorScheduleDTO schedule);
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime date);
        Task<IEnumerable<DoctorScheduleDTO>> GetDoctorSchedulesAsync(int doctorId);
        Task<bool> DeleteScheduleAsync(int scheduleId);
    }
}
