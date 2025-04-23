using Domain.Entities.AppointmentEntities;
using Shared.AppointmentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAppointmentService
    {
        // Get methods
        Task<AppointmentDTO> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDTO>> GetUserAppointmentsAsync(string userId);
        Task<IEnumerable<AppointmentDTO>> GetPetAppointmentsAsync(int petId);
        Task<IEnumerable<AppointmentDTO>> GetDoctorAppointmentsAsync(int doctorId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AppointmentDTO>> GetClinicAppointmentsAsync(int clinicId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        
        // Create method
        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model, string userId);
        
        // Update methods
        Task<AppointmentDTO> UpdateAppointmentStatusAsync(int id, AppointmentDTO model);
        
        // Delete method
        Task DeleteAppointmentAsync(int id);
        
        // Check availability
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime);
    }
}
