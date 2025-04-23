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
        
        // Constructor for finding by doctor ID and day
        public DoctorScheduleSpecification(int doctorId, DayOfWeek day)
            : base(ds => ds.DoctorId == doctorId && ds.Day == day)
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
    }
} 