using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAppointmentService
    {
        Task<AppointmentDTO> GetByIdAsync(Guid id);
        Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByPetAsync(Guid petId, int pageIndex, int pageSize);
        Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByClinicAsync(Guid clinicId, int pageIndex, int pageSize);
        Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByStatusAsync(string status, int pageIndex, int pageSize);
        Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentCreateDTO appointmentCreateDto);
        Task<AppointmentDTO> UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO appointmentUpdateDto);
        Task<AppointmentDTO> UpdateAppointmentStatusAsync(Guid id, string status);
        Task DeleteAppointmentAsync(Guid id);
        Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime appointmentTime, int durationMinutes);
    }
}
