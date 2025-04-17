using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class PrescriptionFilterSpecification : BaseSpecification<Prescription>
    {
        public PrescriptionFilterSpecification()
            : base(null)
        {
            AddIncludes();
            ApplyOrderByDescending(p => p.IssuedDate);
        }

        public PrescriptionFilterSpecification(Guid medicalRecordId)
            : base(p => p.Id == medicalRecordId)
        {
            AddIncludes();
        }

        public PrescriptionFilterSpecification(string medicationName)
            : base(p => p.MedicationItems.Any(mi => mi.Medication.Name.Contains(medicationName)))
        {
            AddIncludes();
        }

        public PrescriptionFilterSpecification(DateTime startDate, DateTime endDate)
            : base(p => p.IssuedDate >= startDate && p.IssuedDate <= endDate)
        {
            AddIncludes();
            ApplyOrderByDescending(p => p.IssuedDate);
        }

        public PrescriptionFilterSpecification(Expression<Func<Prescription, bool>> criteria)
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
