using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class PrescriptionSpecification : BaseSpecification<Prescription>
    {
        public PrescriptionSpecification(Guid id) 
            : base(p => p.Id == id)
        {
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
            AddInclude("MedicationItems.Medication");
        }

        public PrescriptionSpecification(string prescriptionNumber)
            : base(p => p.PrescriptionNumber == prescriptionNumber)
        {
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
            AddInclude("MedicationItems.Medication");
        }

        public PrescriptionSpecification(Guid doctorId, int pageIndex, int pageSize)
            : base(p => p.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
        }

        public PrescriptionSpecification(Guid petProfileId, bool forPet, int pageIndex, int pageSize)
            : base(p => p.PetProfileId == petProfileId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
        }

        public PrescriptionSpecification(string status, int pageIndex, int pageSize)
            : base(p => p.Status == status)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
        }

        public PrescriptionSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(p => p.IssuedDate >= startDate && p.IssuedDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
        }

        public PrescriptionSpecification(Expression<Func<Prescription, bool>> criteria)
            : base(criteria)
        {
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
            AddInclude(p => p.MedicationItems);
        }

        public PrescriptionSpecification()
            : base(null)
        {
            AddOrderByDescending(p => p.IssuedDate);
            AddInclude(p => p.Doctor);
            AddInclude(p => p.PetProfile);
        }
    }
} 