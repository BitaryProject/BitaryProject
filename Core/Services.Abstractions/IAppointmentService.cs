using Shared.AppointmentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAppointmentService
    {
        Task<AppointmentDTO?> GetAppointmentByIdAsync(Guid appointmentId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByUserIdAsync(string userId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPetIdAsync(Guid petId);

        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model);
        Task<AppointmentDTO?> UpdateAppointmentAsync(Guid appointmentId, AppointmentDTO model);

        Task<bool> DeleteAppointmentAsync(Guid appointmentId);
    }
}
