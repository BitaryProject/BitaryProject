using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;

namespace Services.Specifications
{
    public class DateRangePrescriptionSpecification : BaseSpecification<Prescription>
    {
        public DateRangePrescriptionSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(p => p.IssuedDate >= startDate && p.IssuedDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
            AddInclude("MedicationItems.Medication");
        }
    }
} 