using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ClinicEntities;
using Domain.Entities.SecurityEntities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;
using Services.Specifications;
using Shared.RatingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClinicService _clinicService;
        private readonly UserManager<User> _userManager;

        public RatingService(
            IUnitOFWork unitOfWork, 
            IMapper mapper, 
            IClinicService clinicService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clinicService = clinicService;
            _userManager = userManager;
        }

        public async Task<RatingDTO> GetRatingByIdAsync(int id)
        {
            var spec = new RatingSpecification(id);
            var rating = await _unitOfWork.GetRepository<Rating, int>().GetAsync(spec);
            
            if (rating == null)
                throw new RatingNotFoundException(id.ToString());
                
            var ratingDto = _mapper.Map<RatingDTO>(rating);
            
            // Get user's name
            var user = await _userManager.FindByIdAsync(rating.UserId);
            if (user != null)
            {
                ratingDto = ratingDto with { UserName = $"{user.FirstName} {user.LastName}".Trim() };
            }
            
            return ratingDto;
        }

        public async Task<IEnumerable<RatingDTO>> GetRatingsByClinicIdAsync(int clinicId)
        {
            var spec = new RatingSpecification(clinicId, true);
            var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
            
            var ratingDtos = _mapper.Map<IEnumerable<RatingDTO>>(ratings).ToList();
            
            // Get user names for all ratings
            for (int i = 0; i < ratingDtos.Count; i++)
            {
                var userId = ratingDtos[i].UserId;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ratingDtos[i] = ratingDtos[i] with { UserName = $"{user.FirstName} {user.LastName}".Trim() };
                }
            }
            
            return ratingDtos;
        }

        public async Task<IEnumerable<RatingDTO>> GetRatingsByUserIdAsync(string userId)
        {
            var spec = new RatingSpecification(userId);
            var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
            
            var ratingDtos = _mapper.Map<IEnumerable<RatingDTO>>(ratings).ToList();
            
            // Get user name once since all ratings are from the same user
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                string userName = $"{user.FirstName} {user.LastName}".Trim();
                
                // Update all DTOs with the user name
                for (int i = 0; i < ratingDtos.Count; i++)
                {
                    ratingDtos[i] = ratingDtos[i] with { UserName = userName };
                }
            }
            
            return ratingDtos;
        }

        public async Task<bool> HasUserRatedClinicAsync(string userId, int clinicId)
        {
            var spec = new RatingSpecification(userId, clinicId);
            var rating = await _unitOfWork.GetRepository<Rating, int>().GetAsync(spec);
            
            return rating != null;
        }

        public async Task<RatingDTO> CreateRatingAsync(RatingCreateDTO model, string userId)
        {
            // Check if the clinic exists
            var clinic = await _clinicService.GetClinicByIdAsync(model.ClinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(model.ClinicId.ToString());
                
            // Check if the user has already rated this clinic
            if (await HasUserRatedClinicAsync(userId, model.ClinicId))
                throw new InvalidOperationException("You have already rated this clinic. Please update your existing rating instead.");
                
            // Create a new rating
            var rating = _mapper.Map<Rating>(model);
            rating.UserId = userId;
            rating.CreatedAt = DateTime.UtcNow;
            
            await _unitOfWork.GetRepository<Rating, int>().AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();
            
            // Update the clinic's average rating
            await UpdateClinicAverageRatingAsync(model.ClinicId);
            
            // Return the created rating with relationships
            return await GetRatingByIdAsync(rating.Id);
        }

        public async Task<RatingDTO> UpdateRatingAsync(int id, RatingUpdateDTO model, string userId)
        {
            var spec = new RatingSpecification(id);
            var rating = await _unitOfWork.GetRepository<Rating, int>().GetAsync(spec);
            
            if (rating == null)
                throw new RatingNotFoundException(id.ToString());
                
            // Make sure the user is updating their own rating
            if (rating.UserId != userId)
                throw new InvalidOperationException("You can only update your own ratings.");
                
            // Update the rating
            rating.RatingValue = model.RatingValue;
            rating.Comment = model.Comment;
            
            _unitOfWork.GetRepository<Rating, int>().Update(rating);
            await _unitOfWork.SaveChangesAsync();
            
            // Update the clinic's average rating
            await UpdateClinicAverageRatingAsync(rating.ClinicId);
            
            // Return the updated rating with relationships
            return await GetRatingByIdAsync(id);
        }

        public async Task<bool> DeleteRatingAsync(int id, string userId)
        {
            var rating = await _unitOfWork.GetRepository<Rating, int>().GetAsync(id);
            
            if (rating == null)
                throw new RatingNotFoundException(id.ToString());
                
            // Make sure the user is deleting their own rating
            if (rating.UserId != userId)
                throw new InvalidOperationException("You can only delete your own ratings.");
                
            int clinicId = rating.ClinicId;
            
            _unitOfWork.GetRepository<Rating, int>().Delete(rating);
            await _unitOfWork.SaveChangesAsync();
            
            // Update the clinic's average rating
            await UpdateClinicAverageRatingAsync(clinicId);
            
            return true;
        }

        public async Task<IEnumerable<RatingDTO>> GetTopRatingsAsync(int limit = 10)
        {
            var spec = RatingSpecification.GetTopRatings(limit);
            var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
            
            var ratingDtos = _mapper.Map<IEnumerable<RatingDTO>>(ratings).ToList();
            
            // Get user names for all ratings
            for (int i = 0; i < ratingDtos.Count; i++)
            {
                var userId = ratingDtos[i].UserId;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ratingDtos[i] = ratingDtos[i] with { UserName = $"{user.FirstName} {user.LastName}".Trim() };
                }
            }
            
            return ratingDtos;
        }

        public async Task<IEnumerable<RatingDTO>> GetLatestRatingsAsync(int limit = 10)
        {
            var spec = RatingSpecification.GetLatestRatings(limit);
            var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
            
            var ratingDtos = _mapper.Map<IEnumerable<RatingDTO>>(ratings).ToList();
            
            // Get user names for all ratings
            for (int i = 0; i < ratingDtos.Count; i++)
            {
                var userId = ratingDtos[i].UserId;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ratingDtos[i] = ratingDtos[i] with { UserName = $"{user.FirstName} {user.LastName}".Trim() };
                }
            }
            
            return ratingDtos;
        }

        public async Task<double> CalculateAverageRatingAsync(int clinicId)
        {
            var spec = new RatingSpecification(clinicId, true);
            var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
            
            if (!ratings.Any())
                return 0;
                
            return ratings.Average(r => r.RatingValue);
        }
        
        private async Task UpdateClinicAverageRatingAsync(int clinicId)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic != null)
            {
                // Calculate the new average rating
                var spec = new RatingSpecification(clinicId, true);
                var ratings = await _unitOfWork.GetRepository<Rating, int>().GetAllAsync(spec);
                
                clinic.RatingCount = ratings.Count();
                clinic.Rating = ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
                
                _unitOfWork.GetRepository<Clinic, int>().Update(clinic);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
} 