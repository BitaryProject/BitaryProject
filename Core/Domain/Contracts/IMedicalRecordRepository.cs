using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord, Guid>
    {
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetIdAsync(Guid petId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDiagnosisAsync(string diagnosis);
        Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(ISpecification<MedicalRecord> specification, int pageIndex, int pageSize);
        Task<bool> AddNoteToMedicalRecordAsync(Guid id, string note);
    }
} 

