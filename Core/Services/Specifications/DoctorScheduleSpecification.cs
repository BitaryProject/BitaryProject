using Domain.Contracts;
using Domain.Entities.DoctorEntites;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class DoctorScheduleSpecification : Specifications<DoctorSchedule>
    {
        // Constructor for finding by ID
        public DoctorScheduleSpecification(int scheduleId)
            : base(ds => ds.Id == scheduleId)
        {
            AddInclude(ds => ds.Doctor);
        }
        
        // Constructor for finding by doctor ID and date
        public DoctorScheduleSpecification(int doctorId, DateTime date)
            : base(ds => ds.DoctorId == doctorId && ds.ScheduleDate.Date == date.Date)
        {
        }
        
        // Constructor for finding by doctor ID for a date range
        public DoctorScheduleSpecification(int doctorId, DateTime startDate, DateTime endDate)
            : base(ds => ds.DoctorId == doctorId && ds.ScheduleDate.Date >= startDate.Date && ds.ScheduleDate.Date <= endDate.Date)
        {
        }
        
        // Constructor for finding all schedules by doctor ID
        public DoctorScheduleSpecification(Expression<Func<DoctorSchedule, bool>> criteria)
            : base(criteria)
        {
        }
        
        // Method to create a specification for all schedules by doctor ID
        public static DoctorScheduleSpecification GetByDoctorId(int doctorId)
        {
            return new DoctorScheduleSpecification(ds => ds.DoctorId == doctorId);
        }
        
        // Method to create a specification for schedules by doctor ID for current date and future
        public static DoctorScheduleSpecification GetFutureSchedulesByDoctorId(int doctorId)
        {
            var today = DateTime.Today;
            return new DoctorScheduleSpecification(ds => ds.DoctorId == doctorId && ds.ScheduleDate.Date >= today);
        }
    }
} 