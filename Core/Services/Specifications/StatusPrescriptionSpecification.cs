using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class StatusPrescriptionSpecification : BaseSpecification<Prescription>
    {
        public StatusPrescriptionSpecification(string status, int pageIndex, int pageSize)
            : base(p => p.Status.ToString() == status)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public StatusPrescriptionSpecification(Expression<Func<Prescription, bool>> criteria)
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
