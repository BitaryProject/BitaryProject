using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DoctorEntites
{
    public class DoctorSchedule : BaseEntity<Guid>
    {
        public Guid DoctorId { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Doctor Doctor { get; set; }
    }
}
