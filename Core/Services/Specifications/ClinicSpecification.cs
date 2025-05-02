using Domain.Contracts;
using Domain.Entities.ClinicEntities;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class ClinicSpecification : Specifications<Clinic>
    {
        public ClinicSpecification(int id)
            : base(c => c.Id == id)
        {
            AddInclude(c => c.Doctors);
        }

        public ClinicSpecification(Expression<Func<Clinic, bool>> criteria)
            : base(criteria)
        {
            AddInclude(c => c.Doctors);
        }
    }
}

