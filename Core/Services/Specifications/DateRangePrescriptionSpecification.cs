using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class DateRangePrescriptionSpecification : BaseSpecification<Prescription>
    {
        public DateRangePrescriptionSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(p => p.IssuedDate >= startDate && p.IssuedDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public DateRangePrescriptionSpecification(Expression<Func<Prescription, bool>> criteria)
            : base(criteria)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
            AddInclude("MedicationItems.Medication");
        }
    }
} 
