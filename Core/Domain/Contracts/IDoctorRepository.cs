using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IDoctorRepository : IGenericRepository<Doctor, Guid>
    {
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> GetDoctorsByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime dateTime, TimeSpan duration);
        Task<Doctor> GetDoctorWithDetailsAsync(Guid id);
        Task<Doctor> GetDoctorByUserIdAsync(string userId);
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(ISpecification<Doctor> specification, int pageIndex, int pageSize);
        
        // Rating related methods
        Task<IEnumerable<DoctorRating>> GetRatingsAsync(Guid doctorId);
        Task<DoctorRating> GetRatingByIdAsync(Guid ratingId);
        Task<DoctorRating> GetRatingByUserAsync(Guid doctorId, Guid petOwnerId);
        Task<double> GetAverageRatingAsync(Guid doctorId);
    }
} 

