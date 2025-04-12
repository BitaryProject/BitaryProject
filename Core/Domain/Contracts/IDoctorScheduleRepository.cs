/*using Domain.Entities.DoctorEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IDoctorScheduleRepository
    {
        Task<DoctorSchedule> GetScheduleAsync(Guid doctorId, DayOfWeek day);
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(Guid doctorId);
        Task AddAsync(DoctorSchedule schedule);
        Task UpdateAsync(DoctorSchedule schedule);
        Task DeleteAsync(DoctorSchedule schedule);
        Task<bool> CheckScheduleConflictAsync(Guid doctorId, DayOfWeek day);
        Task SaveChangesAsync();
    }
}
*/