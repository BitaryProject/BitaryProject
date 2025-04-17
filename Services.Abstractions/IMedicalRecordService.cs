using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecordsAsync();
        Task<PagedResultDTO<MedicalRecordDTO>> GetPagedMedicalRecordsAsync(int pageIndex, int pageSize);
        Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(Guid id);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByPetIdAsync(Guid petId);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId);
        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordCreateDTO recordDto);
        Task UpdateMedicalRecordAsync(Guid id, MedicalRecordUpdateDTO recordDto);
        Task DeleteMedicalRecordAsync(Guid id);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis);
        Task AddNoteToMedicalRecordAsync(Guid id, string note);
        Task<PagedResultDTO<MedicalRecordDTO>> SearchMedicalRecordsAsync(string searchTerm, int pageIndex, int pageSize);

        #region Reports and Analytics
        Task<MedicalReportDTO> GeneratePetMedicalReportAsync(Guid petId, DateTime startDate, DateTime endDate);
        Task<DoctorActivityReportDTO> GenerateDoctorActivityReportAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<ClinicActivityReportDTO> GenerateClinicActivityReportAsync(Guid clinicId, DateTime startDate, DateTime endDate);
        Task<HealthcareStatisticsDTO> GetHealthcareStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CommonConditionDTO>> GetCommonConditionsReportAsync(DateTime startDate, DateTime endDate, string species = null, int top = 10);
        Task<IEnumerable<TreatmentEffectivenessDTO>> GetTreatmentEffectivenessReportAsync(string condition, DateTime startDate, DateTime endDate);
        Task<FileExportResultDTO> ExportPetMedicalHistoryAsync(Guid petId, string format = "pdf");
        #endregion
    }
} 