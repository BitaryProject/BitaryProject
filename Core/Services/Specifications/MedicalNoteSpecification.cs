using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class MedicalNoteSpecification : BaseSpecification<MedicalNote>
    {
        public MedicalNoteSpecification()
            : base(null)
        {
            AddIncludes();
        }

        public MedicalNoteSpecification(Guid id)
            : base(n => n.Id == id)
        {
            AddIncludes();
        }

        public MedicalNoteSpecification(Guid medicalRecordId, int pageIndex, int pageSize)
            : base(n => n.MedicalRecordId == medicalRecordId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(n => n.CreatedAt);
        }

        public MedicalNoteSpecification(Guid doctorId, bool isDoctor, int pageIndex, int pageSize)
            : base(n => n.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(n => n.CreatedAt);
        }

        public MedicalNoteSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(n => n.CreatedAt >= startDate && n.CreatedAt <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(n => n.CreatedAt);
        }

        public MedicalNoteSpecification(string content, int pageIndex, int pageSize)
            : base(n => n.Content.Contains(content))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddIncludes();
            ApplyOrderByDescending(n => n.CreatedAt);
        }

        // Constructor for Expression<Func<MedicalNote, bool>> predicate
        public MedicalNoteSpecification(Expression<Func<MedicalNote, bool>> predicate)
            : base(predicate)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(n => n.Doctor);
            AddInclude(n => n.MedicalRecord);
        }
    }
} 
