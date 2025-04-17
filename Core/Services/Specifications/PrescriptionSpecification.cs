using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class PrescriptionSpecification : BaseSpecification<Prescription>
    {
        public PrescriptionSpecification(Guid id) 
            : base(p => p.Id == id)
        {
            AddIncludes();
        }

        public PrescriptionSpecification(string prescriptionNumber)
            : base(p => p.PrescriptionNumber == prescriptionNumber)
        {
            AddIncludes();
        }

        public PrescriptionSpecification(Guid doctorId, int pageIndex, int pageSize)
            : base(p => p.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public PrescriptionSpecification(Guid petProfileId, bool forPet, int pageIndex, int pageSize)
            : base(p => p.PetProfileId == petProfileId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public PrescriptionSpecification(string status, int pageIndex, int pageSize)
            : base(p => p.Status.ToString() == status)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public PrescriptionSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(p => p.IssuedDate >= startDate && p.IssuedDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.IssuedDate);
            AddIncludes();
        }

        public PrescriptionSpecification(Expression<Func<Prescription, bool>> criteria)
            : base(criteria)
        {
            AddIncludes();
        }

        public PrescriptionSpecification()
            : base(null)
        {
            ApplyOrderByDescending(p => p.IssuedDate);
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
