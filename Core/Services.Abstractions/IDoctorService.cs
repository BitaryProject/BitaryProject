/*using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IDoctorService
    {
        Task<DoctorDTO?> GetDoctorByIdAsync(Guid doctorId);
        Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorDTO> CreateDoctorAsync(DoctorDTO model);
        Task<DoctorDTO?> UpdateDoctorAsync(Guid doctorId, DoctorDTO model);
        Task<bool> DeleteDoctorAsync(Guid doctorId);
    }
}
*/