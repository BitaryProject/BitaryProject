using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class DoctorSpecification : BaseSpecification<Doctor>
    {
        public DoctorSpecification(Guid id) 
            : base(d => d.Id == id)
        {
            AddIncludes(true);
        }

        public DoctorSpecification(string userId)
            : base(d => d.UserId == userId)
        {
            AddIncludes(false);
        }

        public DoctorSpecification(string specialization, int pageIndex, int pageSize)
            : base(d => string.IsNullOrEmpty(specialization) || d.Specialization.ToLower().Contains(specialization.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(d => d.FullName);
            AddIncludes(false);
        }

        public DoctorSpecification(Guid clinicId, int pageIndex, int pageSize)
            : base(d => d.ClinicId == clinicId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(d => d.FullName);
            AddIncludes(false);
        }

        public DoctorSpecification(Expression<Func<Doctor, bool>> criteria)
            : base(criteria)
        {
            AddIncludes(false);
        }

        public DoctorSpecification()
            : base(null)
        {
            AddOrderBy(d => d.FullName);
            AddIncludes(false);
        }

        private void AddIncludes(bool includeDetails)
        {
            AddInclude(d => d.Clinic);
            
            if (includeDetails)
            {
                AddInclude(d => d.Appointments);
                AddInclude(d => d.Prescriptions);
            }
        }
    }
}
