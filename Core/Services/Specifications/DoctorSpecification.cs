using Domain.Contracts;
using Domain.Entities.DoctorEntites;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class DoctorSpecification : Specifications<Doctor>
    {
        public DoctorSpecification(int id)
            : base(d => d.Id == id)
        {
            AddInclude(d => d.Clinic);
        }
        
        public DoctorSpecification(string userId)
            : base(d => d.UserId == userId)
        {
        }

        public DoctorSpecification(Expression<Func<Doctor, bool>> criteria)
            : base(criteria)
        {
        }
        
        public DoctorSpecification(string specialty, int pageIndex, int pageSize)
            : base(d => d.Specialty.ToLower().Contains(specialty.ToLower()))
        {
            ApplyPagination(pageIndex, pageSize);
            setOrderBy(d => d.Name);
        }
    }
}
