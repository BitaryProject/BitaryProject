//using Domain.Entities.DoctorEntites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Core.Domain.Contracts
//{
//    public interface IDoctorScheduleRepository
//    {
//        Task<DoctorSchedule> GetScheduleAsync(int doctorId, DayOfWeek day);
//        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId);
//        Task AddAsync(DoctorSchedule schedule);
//        Task UpdateAsync(DoctorSchedule schedule);
//        Task DeleteAsync(DoctorSchedule schedule);
//        Task<bool> CheckScheduleConflictAsync(int doctorId, DayOfWeek day);
//        Task SaveChangesAsync();
//    }
//}

