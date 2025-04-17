using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Specifications
{
    public class ClinicSpecification : BaseSpecification<Clinic>
    {
        public ClinicSpecification(Guid id) 
            : base(c => c.Id == id)
        {
            AddInclude(c => c.Doctors);
        }

        public ClinicSpecification(string name, int pageIndex, int pageSize)
            : base(c => string.IsNullOrEmpty(name) || c.Name.ToLower().Contains(name.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(c => c.Name);
            AddInclude(c => c.Doctors);
        }

        public ClinicSpecification(string address, bool searchByAddress, int pageIndex, int pageSize)
            : base(c => c.Address.ToLower().Contains(address.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(c => c.Name);
            AddInclude(c => c.Doctors);
        }

        public ClinicSpecification(Expression<Func<Clinic, bool>> criteria)
            : base(criteria)
        {
            AddInclude(c => c.Doctors);
        }

        public ClinicSpecification()
            : base(null)
        {
            AddOrderBy(c => c.Name);
            AddInclude(c => c.Doctors);
        }
    }
}

