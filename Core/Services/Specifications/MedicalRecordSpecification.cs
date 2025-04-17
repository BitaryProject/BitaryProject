using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Core.Services.Specifications
{
    public class MedicalRecordSpecification : BaseSpecification<MedicalRecord>
    {
        public MedicalRecordSpecification()
            : base(null)
        {
            AddIncludes();
        }

        public MedicalRecordSpecification(Guid id)
            : base(m => m.Id == id)
        {
            AddIncludes();
        }

        public MedicalRecordSpecification(Guid petId, int pageIndex, int pageSize)
            : base(m => m.PetProfileId == petId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(m => m.RecordDate);
        }

        public MedicalRecordSpecification(Guid doctorId, bool isDoctor, int pageIndex, int pageSize)
            : base(m => m.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(m => m.RecordDate);
        }

        public MedicalRecordSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(m => m.RecordDate >= startDate && m.RecordDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(m => m.RecordDate);
        }

        public MedicalRecordSpecification(string diagnosis, int pageIndex, int pageSize)
            : base(m => m.Diagnosis.Contains(diagnosis))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(m => m.RecordDate);
        }

        // Constructor for Expression<Func<MedicalRecord, bool>> predicate
        public MedicalRecordSpecification(Expression<Func<MedicalRecord, bool>> predicate)
            : base(predicate)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(m => m.Doctor);
            AddInclude(m => m.PetProfile);
        }
      }
}
