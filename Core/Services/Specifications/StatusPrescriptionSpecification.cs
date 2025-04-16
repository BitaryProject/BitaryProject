using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;

namespace Services.Specifications
{
    public class StatusPrescriptionSpecification : BaseSpecification<Prescription>
    {
        public StatusPrescriptionSpecification(string status, int pageIndex, int pageSize)
            : base(p => p.Status == status)
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