using Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDTO> GetByIdAsync(Guid id);
        Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByPetAsync(Guid petId, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis, int pageIndex, int pageSize);
        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordCreateDTO recordCreateDto);
        Task<MedicalRecordDTO> UpdateMedicalRecordAsync(Guid id, MedicalRecordUpdateDTO recordUpdateDto);
        Task DeleteMedicalRecordAsync(Guid id);
    }
}
