using Shared.ClinicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IClinicService
    {
        Task<ClinicDTO?> GetClinicByIdAsync(Guid clinicId);
        Task<IEnumerable<ClinicDTO>> GetAllClinicsAsync();
        Task<ClinicDTO> CreateClinicAsync(ClinicDTO model);
        Task<ClinicDTO?> UpdateClinicAsync(Guid clinicId, ClinicDTO model);
        Task<bool> DeleteClinicAsync(Guid clinicId);
    }
}
