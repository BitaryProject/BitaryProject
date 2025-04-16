using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IDoctorService
    {
        Task<DoctorDTO> GetByIdAsync(Guid id);
        Task<DoctorDTO> GetByUserIdAsync(string userId);
        Task<PagedResultDTO<DoctorDTO>> GetDoctorsBySpecializationAsync(string specialization, int pageIndex, int pageSize);
        Task<PagedResultDTO<DoctorDTO>> GetDoctorsByClinicAsync(Guid clinicId, int pageIndex, int pageSize);
        Task<DoctorDTO> CreateDoctorAsync(DoctorCreateUpdateDTO doctorCreateDto);
        Task<DoctorDTO> UpdateDoctorAsync(Guid id, DoctorCreateUpdateDTO doctorUpdateDto);
        Task DeleteDoctorAsync(Guid id);
        Task<IEnumerable<TimeSlotDTO>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);
    }
}
