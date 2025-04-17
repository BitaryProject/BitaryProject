using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IRatingService
    {
        #region Doctor Ratings
        Task<PagedResultDTO<DoctorRatingDTO>> GetDoctorRatingsAsync(int pageIndex, int pageSize);
        Task<PagedResultDTO<DoctorRatingDTO>> GetDoctorRatingsByDoctorIdAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<DoctorRatingDTO> CreateDoctorRatingAsync(DoctorRatingCreateDTO ratingDto);
        Task<DoctorRatingDTO> UpdateDoctorRatingAsync(Guid id, DoctorRatingUpdateDTO ratingDto);
        Task<bool> DeleteDoctorRatingAsync(Guid id);
        Task<bool> DoctorRatingExistsAsync(Guid doctorId, Guid petOwnerId);
        Task<double> GetAverageDoctorRatingAsync(Guid doctorId);
        Task<DoctorRatingSummaryDTO> GetDoctorRatingSummaryAsync(Guid doctorId);
        Task<DoctorRatingDTO> GetDoctorRatingByIdAsync(Guid id);
        Task<bool> HasPetOwnerHadAppointmentWithDoctorAsync(Guid petOwnerId, Guid doctorId);
        Task FlagInappropriateDoctorRatingAsync(Guid doctorId, Guid ratingId, string reason);
        #endregion

        #region Clinic Ratings
        Task<PagedResultDTO<ClinicRatingDTO>> GetClinicRatingsAsync(int pageIndex, int pageSize);
        Task<PagedResultDTO<ClinicRatingDTO>> GetClinicRatingsByClinicIdAsync(Guid clinicId, int pageIndex, int pageSize);
        Task<ClinicRatingDTO> CreateClinicRatingAsync(ClinicRatingCreateDTO ratingDto);
        Task<ClinicRatingDTO> UpdateClinicRatingAsync(Guid id, ClinicRatingUpdateDTO ratingDto);
        Task<bool> DeleteClinicRatingAsync(Guid id);
        Task<bool> ClinicRatingExistsAsync(Guid clinicId, Guid petOwnerId);
        Task<double> GetAverageClinicRatingAsync(Guid clinicId);
        Task<ClinicRatingSummaryDTO> GetClinicRatingSummaryAsync(Guid clinicId);
        Task<ClinicRatingDTO> GetClinicRatingByIdAsync(Guid id);
        Task<bool> HasPetOwnerHadAppointmentAtClinicAsync(Guid petOwnerId, Guid clinicId);
        Task FlagInappropriateClinicRatingAsync(Guid clinicId, Guid ratingId, string reason);
        #endregion

        #region Analytics and Reports
        Task<IEnumerable<DoctorRatingDTO>> GetTopRatedDoctorsAsync(int limit, string specialty = null);
        Task<IEnumerable<ClinicRatingDTO>> GetTopRatedClinicsAsync(int limit, string city = null);
        Task<IEnumerable<RatingTrendDTO>> GetDoctorRatingTrendsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<RatingTrendDTO>> GetClinicRatingTrendsAsync(Guid clinicId, DateTime startDate, DateTime endDate);
        #endregion

        #region Rating flags and moderation
        Task<bool> FlagDoctorRatingAsync(Guid ratingId, string reason);
        Task<bool> FlagClinicRatingAsync(Guid ratingId, string reason);
        Task<bool> ApproveDoctorRatingAsync(Guid ratingId);
        Task<bool> ApproveClinicRatingAsync(Guid ratingId);
        Task<bool> DenyDoctorRatingAsync(Guid ratingId, string reason);
        Task<bool> DenyClinicRatingAsync(Guid ratingId, string reason);
        #endregion
    }
} 