using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class DoctorSpecification : BaseSpecification<Doctor>
    {
        public DoctorSpecification(Guid id) 
            : base(d => d.Id == id)
        {
            AddInclude(d => d.Clinic);
            AddInclude(d => d.Appointments);
            AddInclude(d => d.Prescriptions);
        }

        public DoctorSpecification(string userId)
            : base(d => d.UserId == userId)
        {
            AddInclude(d => d.Clinic);
        }

        public DoctorSpecification(string specialization, int pageIndex, int pageSize)
            : base(d => string.IsNullOrEmpty(specialization) || d.Specialization.ToLower().Contains(specialization.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(d => d.FullName);
            AddInclude(d => d.Clinic);
        }

        public DoctorSpecification(Guid clinicId, int pageIndex, int pageSize)
            : base(d => d.ClinicId == clinicId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(d => d.FullName);
            AddInclude(d => d.Clinic);
        }

        public DoctorSpecification(Expression<Func<Doctor, bool>> criteria)
            : base(criteria)
        {
            AddInclude(d => d.Clinic);
        }

        public DoctorSpecification()
            : base(null)
        {
            AddOrderBy(d => d.FullName);
            AddInclude(d => d.Clinic);
        }
    }
}
