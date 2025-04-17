using Core.Domain.Entities.HealthcareEntities;
using Shared;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IRatingService
    {
        // Doctor Ratings
        Task<DoctorRatingDTO> GetDoctorRatingByIdAsync(Guid id);
        Task<DoctorRatingDTO> GetDoctorRatingByIdAsync(Guid doctorId, Guid ratingId);
        Task<IEnumerable<DoctorRatingDTO>> GetDoctorRatingsByDoctorIdAsync(Guid doctorId);
        Task<PaginatedResult<DoctorRatingDTO>> GetPagedDoctorRatingsAsync(int pageIndex = 1, int pageSize = 10);
        Task<DoctorRatingDTO> GetDoctorRatingByDoctorAndOwnerAsync(Guid doctorId, Guid petOwnerId);
        Task<DoctorRatingDTO> CreateDoctorRatingAsync(DoctorRatingCreateDTO createDto);
        Task<DoctorRatingDTO> UpdateDoctorRatingAsync(Guid id, DoctorRatingUpdateDTO updateDto);
        Task DeleteDoctorRatingAsync(Guid id);
        Task<double> GetAverageDoctorRatingAsync(Guid doctorId);
        Task<PagedResultDTO<DoctorRatingDTO>> GetDoctorRatingsAsync(Guid doctorId, int pageIndex, int pageSize);
        Task<DoctorRatingSummaryDTO> GetDoctorRatingSummaryAsync(Guid doctorId);
        Task FlagInappropriateDoctorRatingAsync(Guid doctorId, Guid ratingId, string reason);
        
        // Clinic Ratings
        Task<ClinicRatingDTO> GetClinicRatingByIdAsync(Guid id);
        Task<ClinicRatingDTO> GetClinicRatingByIdAsync(Guid clinicId, Guid ratingId);
        Task<IEnumerable<ClinicRatingDTO>> GetClinicRatingsByClinicIdAsync(Guid clinicId);
        Task<PaginatedResult<ClinicRatingDTO>> GetPagedClinicRatingsAsync(int pageIndex = 1, int pageSize = 10);
        Task<ClinicRatingDTO> GetClinicRatingByClinicAndOwnerAsync(Guid clinicId, Guid petOwnerId);
        Task<ClinicRatingDTO> CreateClinicRatingAsync(ClinicRatingCreateDTO createDto);
        Task<ClinicRatingDTO> UpdateClinicRatingAsync(Guid id, ClinicRatingUpdateDTO updateDto);
        Task DeleteClinicRatingAsync(Guid id);
        Task<double> GetAverageClinicRatingAsync(Guid clinicId);
        Task<PagedResultDTO<ClinicRatingDTO>> GetClinicRatingsAsync(Guid clinicId, int pageIndex, int pageSize);
        Task<ClinicRatingSummaryDTO> GetClinicRatingSummaryAsync(Guid clinicId);
        Task FlagInappropriateClinicRatingAsync(Guid clinicId, Guid ratingId, string reason);
        
        // Top-rated entities
        Task<IEnumerable<DoctorWithRatingDTO>> GetTopRatedDoctorsAsync(int count = 10);
        Task<IEnumerable<DoctorWithRatingDTO>> GetTopRatedDoctorsAsync(int count, string specialty);
        Task<IEnumerable<ClinicWithRatingDTO>> GetTopRatedClinicsAsync(int count = 10);
        Task<IEnumerable<ClinicWithRatingDTO>> GetTopRatedClinicsAsync(int count, string city);
        
        // Rating trends
        Task<IEnumerable<RatingTrendDTO>> GetDoctorRatingTrendsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<RatingTrendDTO>> GetClinicRatingTrendsAsync(Guid clinicId, DateTime startDate, DateTime endDate);
    }
} 
