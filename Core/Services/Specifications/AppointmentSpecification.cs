using Domain.Contracts;
using Domain.Entities.AppointmentEntities;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class AppointmentSpecification : Specifications<Appointment>
    {
        public AppointmentSpecification(int id)
            : base(a => a.Id == id)
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
        }

        public AppointmentSpecification(string userId)
            : base(a => a.UserId == userId)
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(int petId, bool isPetId)
            : base(a => a.PetId == petId)
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(int doctorId, bool isDoctorId, DateTime? fromDate = null, DateTime? toDate = null)
            : base(a => a.DoctorId == doctorId && 
                         (!fromDate.HasValue || a.AppointmentDate >= fromDate.Value) &&
                         (!toDate.HasValue || a.AppointmentDate <= toDate.Value))
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(int doctorId, AppointmentStatus status, DateTime? fromDate = null, DateTime? toDate = null)
            : base(a => a.DoctorId == doctorId && 
                         a.Status == status &&
                         (!fromDate.HasValue || a.AppointmentDate >= fromDate.Value) &&
                         (!toDate.HasValue || a.AppointmentDate <= toDate.Value))
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(int clinicId, string clinicParam, DateTime? fromDate = null, DateTime? toDate = null)
            : base(a => a.ClinicId == clinicId && 
                         (!fromDate.HasValue || a.AppointmentDate >= fromDate.Value) &&
                         (!toDate.HasValue || a.AppointmentDate <= toDate.Value))
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(AppointmentStatus status)
            : base(a => a.Status == status)
        {
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
            AddInclude(a => a.Doctor);
            setOrderByDescending(a => a.AppointmentDate);
        }

        public AppointmentSpecification(int doctorId, DateTime date, TimeSpan duration)
            : base(a => a.DoctorId == doctorId &&
                         a.AppointmentDate < date.Add(duration) &&
                         a.AppointmentDate.AddMinutes(30) > date)
        {
        }

        public AppointmentSpecification(Expression<Func<Appointment, bool>> criteria)
            : base(criteria)
        {
        }

        public static AppointmentSpecification ForClinic(int clinicId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return new AppointmentSpecification(a => 
                a.ClinicId == clinicId && 
                (!fromDate.HasValue || a.AppointmentDate >= fromDate.Value) &&
                (!toDate.HasValue || a.AppointmentDate <= toDate.Value));
        }
    }
}
