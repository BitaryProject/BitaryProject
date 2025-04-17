using Core.Domain.Entities.HealthcareEntities;
 using Core.Services.Specifications.Base;
using System.Linq.Expressions;
using System;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Specifications
{
    public class AppointmentSpecification : BaseSpecification<Appointment>
    {
        private void AddStandardIncludes()
        {
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(Guid id) 
            : base(a => a.Id == id)
        {
            AddStandardIncludes();
        }

        public AppointmentSpecification(Guid doctorId, int pageIndex, int pageSize)
            : base(a => a.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }

        public AppointmentSpecification(Guid petId, bool forPet, int pageIndex, int pageSize)
            : base(a => a.PetProfileId == petId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }

        public AppointmentSpecification(string status, int pageIndex, int pageSize)
            : base(a => string.Equals(a.Status.ToString(), status, StringComparison.OrdinalIgnoreCase))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }

        public AppointmentSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(a => a.AppointmentDateTime >= startDate && a.AppointmentDateTime <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }

        public AppointmentSpecification(Guid clinicId, string byClinicParam, int pageIndex, int pageSize)
            : base(a => a.ClinicId == clinicId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }

        public AppointmentSpecification(Expression<Func<Appointment, bool>> criteria)
            : base(criteria)
        {
            AddStandardIncludes();
        }

        public AppointmentSpecification()
            : base(null)
        {
            AddOrderBy(a => a.AppointmentDateTime);
            AddStandardIncludes();
        }
    }
}
