using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IPrescriptionService
    {
        // Main interface methods
        Task<PrescriptionDTO> GetByIdAsync(Guid id);
        Task<PrescriptionDTO> GetByPrescriptionNumberAsync(string prescriptionNumber);
        Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByPetAsync(Guid petId, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByStatusAsync(string status, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
        Task<PrescriptionDTO> CreatePrescriptionAsync(PrescriptionCreateDTO prescriptionCreateDto);
        Task<PrescriptionDTO> UpdatePrescriptionAsync(Guid id, PrescriptionUpdateDTO prescriptionUpdateDto);
        Task<PrescriptionDTO> UpdatePrescriptionStatusAsync(Guid id, string status);
        Task DeletePrescriptionAsync(Guid id);
        Task<bool> IsMedicationSafeForPetAsync(Guid petId, Guid medicationId);
        
        // Legacy methods
        Task<PrescriptionDTO> GetPrescriptionByIdAsync(Guid id);
        Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByPetProfileIdAsync(Guid petProfileId);
        Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<PrescriptionDTO>> GetActivePrescriptionsForPetAsync(Guid petProfileId);
        Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId);
        Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByMedicationAsync(string medicationName, int pageIndex, int pageSize);
    }
} 
