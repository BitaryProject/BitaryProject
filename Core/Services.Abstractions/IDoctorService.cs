using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IDoctorService
    {
        // Basic CRUD operations
        Task<DoctorDTO> GetDoctorByIdAsync(Guid id);
        Task<DoctorDTO> GetDoctorByUserIdAsync(string userId);
        Task<DoctorDTO> CreateDoctorAsync(DoctorCreateDTO createDto);
        Task<DoctorDTO> UpdateDoctorAsync(Guid id, DoctorUpdateDTO updateDto);
        Task<bool> DeleteDoctorAsync(Guid id);
        
        // Query operations
        Task<PagedResultDTO<DoctorDTO>> GetPagedDoctorsAsync(int pageIndex, int pageSize);
        Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorDTO>> GetDoctorsByClinicAsync(Guid clinicId);
        Task<IEnumerable<DoctorDTO>> GetDoctorsByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<IEnumerable<DoctorDTO>> GetAvailableDoctorsAsync(DateTime dateTime);
        Task<IEnumerable<DoctorDTO>> GetAvailableDoctorsAsync(DateTime dateTime, Guid clinicId);
        Task<PagedResultDTO<DoctorDTO>> SearchDoctorsAsync(string searchTerm, int pageIndex, int pageSize);
        
        // Status operations
        Task<DoctorDTO> UpdateDoctorStatusAsync(Guid id, string status);
        Task<DoctorDTO> UpdateDoctorProfileAsync(Guid doctorId, DoctorProfileUpdateDTO model);
        
        // Association operations
        Task<bool> AddClinicAssociationAsync(Guid doctorId, Guid clinicId);
        Task<bool> RemoveClinicAssociationAsync(Guid doctorId, Guid clinicId);
        Task AssociateDoctorWithClinicAsync(Guid doctorId, Guid clinicId);
        Task AssociateDoctorWithClinicAsync(Guid doctorId, int clinicId);
        Task DissociateDoctorFromClinicAsync(Guid doctorId, Guid clinicId);
        Task DissociateDoctorFromClinicAsync(Guid doctorId, int clinicId);
        Task AssignDoctorToClinicAsync(Guid doctorId, Guid clinicId);
        Task<PagedResultDTO<DoctorDTO>> GetDoctorsByClinicAsync(Guid clinicId, int pageIndex, int pageSize);
        Task<PagedResultDTO<DoctorDTO>> GetDoctorsByClinicAsync(int clinicId, int pageIndex, int pageSize);
        
        // Availability
        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime appointmentDateTime, TimeSpan duration);
        Task<bool> IsDoctorAssociatedWithClinicAsync(Guid doctorId, Guid clinicId);
        Task<bool> IsDoctorAssociatedWithClinicAsync(Guid doctorId, int clinicId);
        Task<PagedResultDTO<AvailableTimeSlotDTO>> GetAvailableTimeSlotsAsync(Guid doctorId, Guid clinicId, DateTime fromDate, int pageIndex, int pageSize);
        
        // Specialties
        Task<PagedResultDTO<SpecialtyDTO>> GetSpecialtiesByClinicAsync(Guid clinicId, int pageIndex, int pageSize);
    }
}

