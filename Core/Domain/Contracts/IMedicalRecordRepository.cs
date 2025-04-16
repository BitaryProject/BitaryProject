using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord, Guid>
    {
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetProfileIdAsync(Guid petProfileId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId);
        Task<(IEnumerable<MedicalRecord> MedicalRecords, int TotalCount)> GetPagedMedicalRecordsAsync(Specifications<MedicalRecord> specifications, int pageIndex, int pageSize);
        
        // For backward compatibility
        Task<MedicalRecord> GetByIdAsync(Guid id);
        Task<MedicalRecord> GetEntityWithSpecAsync(ISpecification<MedicalRecord> specification);
        Task<IEnumerable<MedicalRecord>> GetAllWithSpecAsync(ISpecification<MedicalRecord> specification);
    }
} 