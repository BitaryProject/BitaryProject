using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IDoctorRepository : IGenericRepository<Doctor, Guid>
    {
        Task<Doctor> GetDoctorByUserIdAsync(string userId);
        Task<IEnumerable<Doctor>> GetDoctorsByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(Specifications<Doctor> specifications, int pageIndex, int pageSize);
        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime appointmentDateTime);
        
        // For backward compatibility
        Task<Doctor> GetByIdAsync(Guid id);
    }
} 