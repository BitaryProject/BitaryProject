using Core.Common.Specifications;

using Core.Domain.Contracts;

using Core.Domain.Entities.HealthcareEntities;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly HealthcareDbContext _context;

        public RatingRepository(HealthcareDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Doctor Ratings

        public async Task<IEnumerable<DoctorRating>> GetDoctorRatingsAsync(Guid doctorId, int skip, int take)
        {
            return await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .Include(r => r.PetOwner)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetDoctorRatingsCountAsync(Guid doctorId)
        {
            return await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .CountAsync();
        }

        public async Task<DoctorRating> GetDoctorRatingByIdAsync(Guid doctorId, Guid ratingId)
        {
            return await _context.DoctorRatings
                .Include(r => r.PetOwner)
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.DoctorId == doctorId && r.Id == ratingId);
        }

        public async Task<DoctorRating> CreateDoctorRatingAsync(DoctorRating rating)
        {
            await _context.DoctorRatings.AddAsync(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task UpdateDoctorRatingAsync(DoctorRating rating)
        {
            _context.DoctorRatings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorRatingAsync(Guid doctorId, Guid ratingId)
        {
            var rating = await GetDoctorRatingByIdAsync(doctorId, ratingId);
            if (rating != null)
            {
                _context.DoctorRatings.Remove(rating);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double> GetDoctorAverageRatingAsync(Guid doctorId)
        {
            if (!await _context.DoctorRatings.AnyAsync(r => r.DoctorId == doctorId))
            {
                return 0;
            }

            return await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .AverageAsync(r => r.Rating);
        }

        public async Task<Dictionary<int, int>> GetDoctorRatingDistributionAsync(Guid doctorId)
        {
            var ratings = await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            return ratings.ToDictionary(r => r.Rating, r => r.Count);
        }

        public async Task<bool> DoctorRatingExistsAsync(Guid doctorId, Guid petOwnerId)
        {
            return await _context.DoctorRatings
                .AnyAsync(r => r.DoctorId == doctorId && r.PetOwnerId == petOwnerId);
        }

        public async Task<IEnumerable<DoctorRating>> GetDoctorRatingsByPetOwnerAsync(Guid petOwnerId)
        {
            return await _context.DoctorRatings
                .Where(r => r.PetOwnerId == petOwnerId)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasPetOwnerHadAppointmentWithDoctorAsync(Guid petOwnerId, Guid doctorId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.PetOwner.Id == petOwnerId && a.Doctor.Id == doctorId);
        }

        public async Task FlagDoctorRatingAsync(Guid doctorId, Guid ratingId, string reason)
        {
            var rating = await GetDoctorRatingByIdAsync(doctorId, ratingId);
            if (rating != null)
            {
                rating.IsFlagged = true;
                rating.FlagReason = reason;
                await UpdateDoctorRatingAsync(rating);
            }
        }

        #endregion

        #region Clinic Ratings

        public async Task<IEnumerable<ClinicRating>> GetClinicRatingsAsync(Guid clinicId, int skip, int take)
        {
            return await _context.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .Include(r => r.PetOwner)
                .Include(r => r.Clinic)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetClinicRatingsCountAsync(Guid clinicId)
        {
            return await _context.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .CountAsync();
        }

        public async Task<ClinicRating> GetClinicRatingByIdAsync(Guid clinicId, Guid ratingId)
        {
            return await _context.ClinicRatings
                .Include(r => r.PetOwner)
                .Include(r => r.Clinic)
                .FirstOrDefaultAsync(r => r.ClinicId == clinicId && r.Id == ratingId);
        }

        public async Task<ClinicRating> CreateClinicRatingAsync(ClinicRating rating)
        {
            await _context.ClinicRatings.AddAsync(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task UpdateClinicRatingAsync(ClinicRating rating)
        {
            _context.ClinicRatings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClinicRatingAsync(Guid clinicId, Guid ratingId)
        {
            var rating = await GetClinicRatingByIdAsync(clinicId, ratingId);
            if (rating != null)
            {
                _context.ClinicRatings.Remove(rating);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double> GetClinicAverageRatingAsync(Guid clinicId)
        {
            if (!await _context.ClinicRatings.AnyAsync(r => r.ClinicId == clinicId))
            {
                return 0;
            }

            return await _context.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .AverageAsync(r => r.Rating);
        }

        public async Task<Dictionary<int, int>> GetClinicRatingDistributionAsync(Guid clinicId)
        {
            var ratings = await _context.ClinicRatings
                .Where(r => r.ClinicId == clinicId)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            return ratings.ToDictionary(r => r.Rating, r => r.Count);
        }

        public async Task<bool> ClinicRatingExistsAsync(Guid clinicId, Guid petOwnerId)
        {
            return await _context.ClinicRatings
                .AnyAsync(r => r.ClinicId == clinicId && r.PetOwnerId == petOwnerId);
        }

        public async Task<IEnumerable<ClinicRating>> GetClinicRatingsByPetOwnerAsync(Guid petOwnerId)
        {
            return await _context.ClinicRatings
                .Where(r => r.PetOwnerId == petOwnerId)
                .Include(r => r.Clinic)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasPetOwnerHadAppointmentAtClinicAsync(Guid petOwnerId, Guid clinicId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.PetOwner.Id == petOwnerId && a.Clinic.Id == clinicId);
        }

        public async Task FlagClinicRatingAsync(Guid clinicId, Guid ratingId, string reason)
        {
            var rating = await GetClinicRatingByIdAsync(clinicId, ratingId);
            if (rating != null)
            {
                rating.IsFlagged = true;
                rating.FlagReason = reason;
                await UpdateClinicRatingAsync(rating);
            }
        }

        #endregion

        #region Analytics

        public async Task<IEnumerable<DoctorRating>> GetTopRatedDoctorsAsync(int limit, string specialty = null)
        {
            var query = _context.Doctors
                .Where(d => specialty == null || d.Specialty.Name == specialty)
                .Include(d => d.Ratings)
                .Where(d => d.Ratings.Any())
                .Select(d => new
                {
                    Doctor = d,
                    AverageRating = d.Ratings.Average(r => r.Rating),
                    RatingCount = d.Ratings.Count
                })
                .OrderByDescending(d => d.AverageRating)
                .ThenByDescending(d => d.RatingCount)
                .Take(limit);

            var result = await query.ToListAsync();

            // Get a sample rating for each doctor to return (could be improved in the future)
            var topDoctorRatings = new List<DoctorRating>();
            foreach (var item in result)
            {
                var rating = await _context.DoctorRatings
                    .Include(r => r.Doctor)
                    .Include(r => r.PetOwner)
                    .Where(r => r.DoctorId == item.Doctor.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefaultAsync();

                if (rating != null)
                {
                    topDoctorRatings.Add(rating);
                }
            }

            return topDoctorRatings;
        }

        public async Task<IEnumerable<ClinicRating>> GetTopRatedClinicsAsync(int limit, string city = null)
        {
            var query = _context.Clinics
                .Where(c => city == null || c.City == city)
                .Include(c => c.Ratings)
                .Where(c => c.Ratings.Any())
                .Select(c => new
                {
                    Clinic = c,
                    AverageRating = c.Ratings.Average(r => r.Rating),
                    RatingCount = c.Ratings.Count
                })
                .OrderByDescending(c => c.AverageRating)
                .ThenByDescending(c => c.RatingCount)
                .Take(limit);

            var result = await query.ToListAsync();

            // Get a sample rating for each clinic to return (could be improved in the future)
            var topClinicRatings = new List<ClinicRating>();
            foreach (var item in result)
            {
                var rating = await _context.ClinicRatings
                    .Include(r => r.Clinic)
                    .Include(r => r.PetOwner)
                    .Where(r => r.ClinicId == item.Clinic.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefaultAsync();

                if (rating != null)
                {
                    topClinicRatings.Add(rating);
                }
            }

            return topClinicRatings;
        }

        public async Task<IEnumerable<RatingTrend>> GetDoctorRatingTrendsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            // Group ratings by month and calculate average
            var trends = await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId && r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                .GroupBy(r => new { Year = r.CreatedAt.Year, Month = r.CreatedAt.Month })
                .Select(g => new RatingTrend
                {
                    EntityType = "Doctor",
                    EntityId = doctorId,
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    AverageRating = g.Average(r => r.Rating),
                    TotalRatings = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return trends;
        }

        public async Task<IEnumerable<RatingTrend>> GetClinicRatingTrendsAsync(Guid clinicId, DateTime startDate, DateTime endDate)
        {
            // Group ratings by month and calculate average
            var trends = await _context.ClinicRatings
                .Where(r => r.ClinicId == clinicId && r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                .GroupBy(r => new { Year = r.CreatedAt.Year, Month = r.CreatedAt.Month })
                .Select(g => new RatingTrend
                {
                    EntityType = "Clinic",
                    EntityId = clinicId,
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    AverageRating = g.Average(r => r.Rating),
                    TotalRatings = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return trends;
        }

        #endregion
    }
} 










