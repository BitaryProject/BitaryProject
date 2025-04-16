using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IClinicService
    {
        Task<ClinicDTO> GetByIdAsync(Guid id);
        Task<PagedResultDTO<ClinicDTO>> GetClinicsByNameAsync(string name, int pageIndex, int pageSize);
        Task<PagedResultDTO<ClinicDTO>> GetClinicsByAddressAsync(string address, int pageIndex, int pageSize);
        Task<PagedResultDTO<ClinicDTO>> GetAllClinicsAsync(int pageIndex, int pageSize);
        Task<ClinicDTO> CreateClinicAsync(ClinicCreateUpdateDTO clinicCreateDto);
        Task<ClinicDTO> UpdateClinicAsync(Guid id, ClinicCreateUpdateDTO clinicUpdateDto);
        Task DeleteClinicAsync(Guid id);
    }
}
