//using Domain.Entities.DoctorEntites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Persistence.Repositories.NewModule
//{
//    public class DoctorScheduleRepository : IDoctorScheduleRepository
//    {
//        private readonly NewModuleContext _context;
//        private readonly DbSet<DoctorSchedule> _dbSet;

//        public DoctorScheduleRepository(NewModuleContext context)
//        {
//            _context = context;
//            _dbSet = _context.Set<DoctorSchedule>();
//        }

//        public async Task<DoctorSchedule> GetScheduleAsync(int doctorId, DayOfWeek day)
//        {
//            return await _dbSet
//                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.Day == day);
//        }

//        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId)
//        {
//            return await _dbSet
//                .Where(s => s.DoctorId == doctorId)
//                .OrderBy(s => s.Day)
//                .ToListAsync();
//        }

//        public async Task AddAsync(DoctorSchedule schedule)
//        {
//            await _dbSet.AddAsync(schedule);
//        }

//        public async Task UpdateAsync(DoctorSchedule schedule)
//        {
//            _dbSet.Update(schedule);
//            await Task.CompletedTask;
//        }

//        public async Task DeleteAsync(DoctorSchedule schedule)
//        {
//            _dbSet.Remove(schedule);
//            await Task.CompletedTask;
//        }

//        public async Task<bool> CheckScheduleConflictAsync(int doctorId, DayOfWeek day)
//        {
//            return await _dbSet
//                .AnyAsync(s => s.DoctorId == doctorId && s.Day == day);
//        }

//        public async Task SaveChangesAsync()
//        {
//            await _context.SaveChangesAsync();
//        }
//    }
//}
