using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class MedicalRecordSpecification : BaseSpecification<MedicalRecord>
    {
        public MedicalRecordSpecification(Guid id) 
            : base(m => m.Id == id)
        {
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }

        public MedicalRecordSpecification(Guid petId, int pageIndex, int pageSize)
            : base(m => m.PetProfileId == petId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(m => m.RecordDate);
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
        }

        public MedicalRecordSpecification(Guid doctorId, bool byDoctor, int pageIndex, int pageSize)
            : base(m => m.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(m => m.RecordDate);
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }

        public MedicalRecordSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(m => m.RecordDate >= startDate && m.RecordDate <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(m => m.RecordDate);
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }

        public MedicalRecordSpecification(string diagnosis, int pageIndex, int pageSize)
            : base(m => m.Diagnosis.ToLower().Contains(diagnosis.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderByDescending(m => m.RecordDate);
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }

        public MedicalRecordSpecification(Expression<Func<MedicalRecord, bool>> criteria)
            : base(criteria)
        {
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }

        public MedicalRecordSpecification()
            : base(null)
        {
            AddOrderByDescending(m => m.RecordDate);
            AddInclude(m => m.PetProfile);
            AddInclude(m => m.Doctor);
            AddInclude("PetProfile.Owner");
        }
    }
}
