using Shared.RatingModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IRatingService
    {
        Task<RatingDTO> GetRatingByIdAsync(int id);
        
        Task<IEnumerable<RatingDTO>> GetRatingsByClinicIdAsync(int clinicId);
        
        Task<IEnumerable<RatingDTO>> GetRatingsByUserIdAsync(string userId);
        
        Task<bool> HasUserRatedClinicAsync(string userId, int clinicId);
        
        Task<RatingDTO> CreateRatingAsync(RatingCreateDTO model, string userId);
        
        Task<RatingDTO> UpdateRatingAsync(int id, RatingUpdateDTO model, string userId);
        
        Task<bool> DeleteRatingAsync(int id, string userId);
        
        Task<IEnumerable<RatingDTO>> GetTopRatingsAsync(int limit = 10);
        
        Task<IEnumerable<RatingDTO>> GetLatestRatingsAsync(int limit = 10);
        
        Task<double> CalculateAverageRatingAsync(int clinicId);
    }
} 