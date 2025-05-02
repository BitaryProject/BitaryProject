using Shared.DoctorModels;
using Shared.ClinicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IDoctorService
    {
        Task<DoctorDTO?> GetDoctorByIdAsync(int doctorId);
        Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorDTO> CreateDoctorAsync(DoctorDTO model, string userId);
        Task<DoctorDTO?> UpdateDoctorAsync(int doctorId, DoctorDTO model);
        Task<bool> DeleteDoctorAsync(int doctorId);
        Task<DoctorDTO?> GetDoctorByUserIdAsync(string userId);
        Task<IEnumerable<ClinicDTO>> GetDoctorClinicsAsync(int doctorId);
    }
}
