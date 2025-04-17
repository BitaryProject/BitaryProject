using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IMedicalNoteRepository : IGenericRepository<MedicalNote, Guid>
    {
        Task<IEnumerable<MedicalNote>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId);
        Task<IEnumerable<MedicalNote>> GetNotesByDoctorIdAsync(Guid doctorId);
        Task<(IEnumerable<MedicalNote> Notes, int TotalCount)> GetPagedNotesAsync(ISpecification<MedicalNote> specification, int pageIndex, int pageSize);
    }
} 

