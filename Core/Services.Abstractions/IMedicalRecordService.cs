using Core.Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecordsAsync();
        Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(Guid id);
        Task<MedicalRecordDTO> GetByIdAsync(Guid id);
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetPagedMedicalRecordsAsync(int pageIndex, int pageSize);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByPetIdAsync(Guid petId);
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByPetAsync(Guid petId, int pageIndex, int pageSize);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId);
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<bool> AddNoteToMedicalRecordAsync(Guid recordId, string note);
        Task<byte[]> ExportPetMedicalHistoryAsync(Guid petId);
        Task<Report> GeneratePetMedicalReportAsync(Guid petId);
        Task<Report> GenerateDoctorActivityReportAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<Report> GenerateClinicActivityReportAsync(Guid clinicId, DateTime startDate, DateTime endDate);
        Task<Statistics> GetHealthcareStatisticsAsync();
        Task<Report> GetCommonConditionsReportAsync();
        Task<Report> GetTreatmentEffectivenessReportAsync();
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> SearchMedicalRecordsAsync(string searchTerm, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis, int pageIndex, int pageSize);
        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordCreateDTO recordDto);
        Task<MedicalRecordDTO> UpdateMedicalRecordAsync(Guid id, MedicalRecordUpdateDTO recordDto);
        Task<bool> DeleteMedicalRecordAsync(Guid id);
        Task<PetMedicalHistoryDTO> GetPetMedicalHistoryAsync(Guid petId);
        Task<PetMedicalReportDTO> GetPetMedicalReportAsync(Guid petId);
        
        Task<MedicalNoteDTO> GetMedicalNoteByIdAsync(Guid id);
        Task<IEnumerable<MedicalNoteDTO>> GetMedicalNotesByRecordIdAsync(Guid recordId);
        Task<HealthcarePagedResultDTO<MedicalNoteDTO>> GetPagedMedicalNotesByRecordIdAsync(Guid recordId, int pageIndex, int pageSize);
        Task<MedicalNoteDTO> CreateMedicalNoteAsync(MedicalNoteCreateDTO noteDto);
        Task<MedicalNoteDTO> UpdateMedicalNoteAsync(Guid id, MedicalNoteUpdateDTO noteDto);
        Task<bool> DeleteMedicalNoteAsync(Guid id);
    }
}

