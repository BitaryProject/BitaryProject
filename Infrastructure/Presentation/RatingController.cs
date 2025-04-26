using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.RatingModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities.SecurityEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public RatingController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Rating/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingDTO>> GetRating(int id)
        {
            try
            {
                var rating = await _serviceManager.RatingService.GetRatingByIdAsync(id);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Rating/clinic/{clinicId}
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetRatingsByClinic(int clinicId)
        {
            var ratings = await _serviceManager.RatingService.GetRatingsByClinicIdAsync(clinicId);
            return Ok(ratings);
        }

        // GET: api/Rating/user
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetUserRatings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ratings = await _serviceManager.RatingService.GetRatingsByUserIdAsync(userId);
            return Ok(ratings);
        }

        // GET: api/Rating/check/{clinicId}
        [HttpGet("check/{clinicId}")]
        [Authorize]
        public async Task<ActionResult<bool>> HasUserRatedClinic(int clinicId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasRated = await _serviceManager.RatingService.HasUserRatedClinicAsync(userId, clinicId);
            return Ok(hasRated);
        }
        
        // GET: api/Rating/top
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetTopRatings([FromQuery] int limit = 10)
        {
            var ratings = await _serviceManager.RatingService.GetTopRatingsAsync(limit);
            return Ok(ratings);
        }

        // GET: api/Rating/latest
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetLatestRatings([FromQuery] int limit = 10)
        {
            var ratings = await _serviceManager.RatingService.GetLatestRatingsAsync(limit);
            return Ok(ratings);
        }

        // GET: api/Rating/average/{clinicId}
        [HttpGet("average/{clinicId}")]
        public async Task<ActionResult<double>> GetAverageRating(int clinicId)
        {
            var average = await _serviceManager.RatingService.CalculateAverageRatingAsync(clinicId);
            return Ok(average);
        }

        // POST: api/Rating
        [HttpPost]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> CreateRating([FromBody] RatingCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var rating = await _serviceManager.RatingService.CreateRatingAsync(model, userId);
                return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Rating/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<RatingDTO>> UpdateRating(int id, [FromBody] RatingUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var rating = await _serviceManager.RatingService.UpdateRatingAsync(id, model, userId);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                    
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Rating/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult> DeleteRating(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole(Role.Admin.ToString());
                
                // If admin, allow deletion of any rating
                if (isAdmin)
                {
                    var rating = await _serviceManager.RatingService.GetRatingByIdAsync(id);
                    userId = rating.UserId; // Use the rating's actual user ID
                }
                
                await _serviceManager.RatingService.DeleteRatingAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                    
                return BadRequest(ex.Message);
            }
        }
    }
} 