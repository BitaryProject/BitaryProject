using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMedicationService
    {
        Task<MedicationDTO> GetByIdAsync(Guid id);
        Task<PagedResultDTO<MedicationDTO>> GetMedicationsByNameAsync(string name, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicationDTO>> GetMedicationsByDosageFormAsync(string dosageForm, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicationDTO>> GetMedicationsByManufacturerAsync(string manufacturer, int pageIndex, int pageSize);
        Task<PagedResultDTO<MedicationDTO>> GetMedicationsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageIndex, int pageSize);
        Task<MedicationDTO> CreateMedicationAsync(MedicationCreateDTO medicationCreateDto);
        Task<MedicationDTO> UpdateMedicationAsync(Guid id, MedicationUpdateDTO medicationUpdateDto);
        Task DeleteMedicationAsync(Guid id);
    }
} 