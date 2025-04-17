using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IClinicService
    {
        // Basic CRUD operations
        Task<ClinicDTO> GetClinicByIdAsync(Guid id);
        Task<ClinicDTO> GetClinicByIdAsync(int id);
        Task<ClinicDTO> CreateClinicAsync(ClinicCreateDTO clinicDto);
        Task<ClinicDTO> UpdateClinicAsync(Guid id, ClinicUpdateDTO clinicDto);
        Task DeleteClinicAsync(Guid id);
        
        // Query operations
        Task<IEnumerable<ClinicDTO>> GetAllClinicsAsync();
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetPagedClinicsAsync(int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetClinicsByLocationAsync(string location, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<ClinicDTO>> SearchClinicsAsync(string searchTerm, int pageIndex, int pageSize);
        
        // Additional query operations
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetClinicsByCityAsync(string city, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetClinicsByPostalCodeAsync(string postalCode, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetClinicsByStatusAsync(string status, int pageIndex, int pageSize);
        
        // Status operations
        Task<ClinicDTO> UpdateClinicStatusAsync(Guid id, string status);
        
        // Association operations
        Task<HealthcarePagedResultDTO<ClinicDTO>> GetClinicsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<bool> IsDoctorAssociatedWithClinicAsync(Guid doctorId, Guid clinicId);
        Task<IEnumerable<DoctorDTO>> GetClinicDoctorsAsync(Guid clinicId);
        Task<bool> AddDoctorToClinicAsync(Guid clinicId, Guid doctorId);
        Task<bool> RemoveDoctorFromClinicAsync(Guid clinicId, Guid doctorId);
        
        // Location operations
        Task<IEnumerable<ClinicDTO>> GetNearestClinicsAsync(double latitude, double longitude, double radiusInKm);
        Task<IEnumerable<ClinicDTO>> GetNearbyClinicsAsync(double latitude, double longitude, int maxResults = 10);
        
        // Authorization
        Task<bool> IsUserClinicAdminAsync(Guid userId, int clinicId);
        Task<bool> IsUserClinicAdminAsync(Guid userId, Guid clinicId);
        
        // Visit verification
        Task<bool> HasPetOwnerVisitedClinicAsync(Guid petOwnerId, Guid clinicId);
    }
}

