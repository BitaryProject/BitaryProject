using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IRatingRepository
    {
        #region Doctor Ratings
        Task<IEnumerable<DoctorRating>> GetDoctorRatingsAsync(Guid doctorId, int skip, int take);
        Task<int> GetDoctorRatingsCountAsync(Guid doctorId);
        Task<DoctorRating> GetDoctorRatingByIdAsync(Guid doctorId, Guid ratingId);
        Task<DoctorRating> CreateDoctorRatingAsync(DoctorRating rating);
        Task UpdateDoctorRatingAsync(DoctorRating rating);
        Task DeleteDoctorRatingAsync(Guid doctorId, Guid ratingId);
        Task<double> GetDoctorAverageRatingAsync(Guid doctorId);
        Task<Dictionary<int, int>> GetDoctorRatingDistributionAsync(Guid doctorId);
        Task<bool> DoctorRatingExistsAsync(Guid doctorId, Guid petOwnerId);
        Task<IEnumerable<DoctorRating>> GetDoctorRatingsByPetOwnerAsync(Guid petOwnerId);
        Task<bool> HasPetOwnerHadAppointmentWithDoctorAsync(Guid petOwnerId, Guid doctorId);
        Task FlagDoctorRatingAsync(Guid doctorId, Guid ratingId, string reason);
        #endregion

        #region Clinic Ratings
        Task<IEnumerable<ClinicRating>> GetClinicRatingsAsync(Guid clinicId, int skip, int take);
        Task<int> GetClinicRatingsCountAsync(Guid clinicId);
        Task<ClinicRating> GetClinicRatingByIdAsync(Guid clinicId, Guid ratingId);
        Task<ClinicRating> CreateClinicRatingAsync(ClinicRating rating);
        Task UpdateClinicRatingAsync(ClinicRating rating);
        Task DeleteClinicRatingAsync(Guid clinicId, Guid ratingId);
        Task<double> GetClinicAverageRatingAsync(Guid clinicId);
        Task<Dictionary<int, int>> GetClinicRatingDistributionAsync(Guid clinicId);
        Task<bool> ClinicRatingExistsAsync(Guid clinicId, Guid petOwnerId);
        Task<IEnumerable<ClinicRating>> GetClinicRatingsByPetOwnerAsync(Guid petOwnerId);
        Task<bool> HasPetOwnerHadAppointmentAtClinicAsync(Guid petOwnerId, Guid clinicId);
        Task FlagClinicRatingAsync(Guid clinicId, Guid ratingId, string reason);
        #endregion

        #region Analytics
        Task<IEnumerable<DoctorRating>> GetTopRatedDoctorsAsync(int limit, string specialty = null);
        Task<IEnumerable<ClinicRating>> GetTopRatedClinicsAsync(int limit, string city = null);
        Task<IEnumerable<RatingTrend>> GetDoctorRatingTrendsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<RatingTrend>> GetClinicRatingTrendsAsync(Guid clinicId, DateTime startDate, DateTime endDate);
        #endregion
    }
} 