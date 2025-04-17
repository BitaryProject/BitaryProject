using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IAppointmentService
    {
        // Get appointment by ID
        Task<AppointmentDTO> GetAppointmentByIdAsync(Guid id);
        
        // Get appointments for a specific pet
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPetIdAsync(Guid petId);
        
        // Get appointments for a specific doctor
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorIdAsync(Guid doctorId, DateTime? fromDate = null);
        
        // Get appointments for a specific clinic
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByClinicIdAsync(Guid clinicId, DateTime? fromDate = null);
        
        // Get appointments for a specific pet owner
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByOwnerIdAsync(Guid ownerId, DateTime? fromDate = null);
        
        // Get all upcoming appointments for a specific pet
        Task<IEnumerable<AppointmentDTO>> GetUpcomingAppointmentsByPetIdAsync(Guid petId);
        
        // Get paged appointments for a specific doctor
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        
        // Get paged appointments for a specific pet
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByPetAsync(Guid petId, int pageIndex, int pageSize);
        
        // Get paged appointments for a specific clinic
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByClinicAsync(Guid clinicId, int pageIndex, int pageSize);
        
        // Get paged appointments by status
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByStatusAsync(string status, int pageIndex, int pageSize);
        
        // Get paged appointments by date range
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
        
        // Create a new appointment
        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentCreateDTO model);
        
        // Update an appointment
        Task<AppointmentDTO> UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO model);
        
        // Update appointment status
        Task<AppointmentDTO> UpdateAppointmentStatusAsync(Guid id, string status);
        
        // Delete an appointment
        Task DeleteAppointmentAsync(Guid id);
        
        // Cancel an appointment
        Task<bool> CancelAppointmentAsync(Guid id, string cancellationReason);
        
        // Check if a time slot is available
        Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime appointmentTime, TimeSpan duration);
        
        // Check if a pet is owned by the specified user
        Task<bool> IsPetOwnedByUserAsync(Guid petId, string userId);
        
        // Get all appointments (paginated)
        Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAllAppointmentsAsync(int pageIndex, int pageSize);
    }
}

