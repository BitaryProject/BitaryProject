global using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IClinicRepository : IGenericRepository<Clinic, Guid>
    {
        Task<IEnumerable<Clinic>> GetClinicsByLocationAsync(string city, string state);
        Task<IEnumerable<Clinic>> SearchClinicsAsync(string searchTerm);
        Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(ISpecification<Clinic> specification, int pageIndex, int pageSize);
        Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count);

        Task<IEnumerable<Clinic>> GetClinicsByStatusAsync(string status);
        Task<IEnumerable<Clinic>> GetClinicsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Clinic>> GetClinicsBySearchTermAsync(string searchTerm);

        // Rating related methods
        Task<IEnumerable<ClinicRating>> GetRatingsAsync(Guid clinicId);
        Task<ClinicRating> GetRatingByIdAsync(Guid ratingId);
        Task<ClinicRating> GetRatingByUserAsync(Guid clinicId, Guid petOwnerId);
        Task<IEnumerable<ClinicRating>> GetRatingsByUserIdAsync(Guid petOwnerId);
        Task AddRatingAsync(ClinicRating rating);
        void UpdateRating(ClinicRating rating);
        void DeleteRating(ClinicRating rating);
        Task<double> GetAverageRatingAsync(Guid clinicId);
    }
} 

