using Shared.ClinicModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IClinicService
    {
        Task<ClinicDTO?> GetClinicByIdAsync(int clinicId);
        Task<IEnumerable<ClinicDTO>> GetAllClinicsAsync();
        Task<ClinicDTO> CreateClinicAsync(ClinicRequestDTO model, string userId);
        Task<ClinicDTO?> UpdateClinicAsync(int clinicId, ClinicDTO model);
        Task<ClinicDTO?> UpdateClinicBasicInfoAsync(int clinicId, ClinicUpdateDTO model);
        Task<bool> DeleteClinicAsync(int clinicId);
        Task<IEnumerable<ClinicDTO>> GetPendingClinicsAsync();
        Task<ClinicDTO> UpdateClinicStatusAsync(int clinicId, ClinicStatusUpdateDTO statusUpdate);
        Task<ClinicDTO> ApproveClinicAsync(int clinicId);
        Task<ClinicDTO> RejectClinicAsync(int clinicId);
        Task<IEnumerable<ClinicDTO>> GetClinicsByOwnerIdAsync(string ownerId);
        Task<ClinicDTO> AddDoctorToClinicAsync(int clinicId, int doctorId);
    }
}
